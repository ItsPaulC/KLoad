import { check } from 'k6';
import kafka from 'k6/x/kafka';
import { sleep } from 'k6';
import encoding from 'k6/encoding';
import proto from 'k6/x/proto';

// Configuration parameters
const brokers = ['localhost:9092'];
const topic = 'employee_topic';

// Proto definition from employee.proto
const employeeProtoFile = `
syntax = "proto3";

package KLoad;

message Employee {
    int32 Id = 1;
    string Name = 2;
    string Position = 3;
}
`;

// Test options
export const options = {
    vus: 10, // Number of virtual users
    iterations: 100, // Total number of iterations across all VUs
    thresholds: {
        'kafka_writer_error_count': ['count<1'], // No errors allowed
    },
};

// Create protobuf encoder
const protoCompiler = new proto.Compiler();
const employeeProto = protoCompiler.compile(employeeProtoFile);
const Employee = employeeProto.lookupType('KLoad.Employee');

// Producer setup
const writer = new kafka.Writer({
    brokers: brokers,
    topic: topic,
    autoCreateTopic: true,
});

// Generate a random employee
function generateEmployee(id) {
    const positions = ['Developer', 'Manager', 'QA Engineer', 'DevOps', 'Product Owner'];
    const names = ['John Doe', 'Jane Smith', 'Bob Johnson', 'Alice Williams', 'David Brown'];
    
    return {
        Id: id,
        Name: names[Math.floor(Math.random() * names.length)],
        Position: positions[Math.floor(Math.random() * positions.length)],
    };
}

// Encode employee object using protobuf
function encodeEmployee(employee) {
    // Verify the object is valid for the message type
    const errMsg = Employee.verify(employee);
    if (errMsg) {
        throw new Error(errMsg);
    }
    
    // Create and encode message
    const message = Employee.create(employee);
    const buffer = Employee.encode(message).finish();
    
    return buffer;
}

// Fallback to JSON if protobuf encoding fails
function employeeToJson(employee) {
    return JSON.stringify(employee);
}

// Main test function
export default function () {
    const vuId = __VU;
    const iterationId = __ITER;
    const employeeId = (vuId * 1000) + iterationId;
    
    // Create an employee
    const employee = generateEmployee(employeeId);
    
    let messageValue;
    let encodingType = 'protobuf';
    
    try {
        // Try to encode with protobuf
        messageValue = encodeEmployee(employee);
    } catch (e) {
        // Fallback to JSON encoding if protobuf fails
        console.error(`Protobuf encoding failed: ${e.message}. Falling back to JSON.`);
        messageValue = employeeToJson(employee);
        encodingType = 'json';
    }
    
    // Log the employee we're sending
    console.log(`Producing message for Employee: Id=${employee.Id}, Name=${employee.Name}, Position=${employee.Position} (${encodingType})`);
    
    // Send the message
    const result = writer.produce({
        value: messageValue,
    });
    
    // Check if the message was sent successfully
    check(result, {
        'message sent': (r) => r.success,
    });
    
    // Add some randomized think time between requests
    sleep(Math.random() * 1);
}

export function teardown() {
    // Close the writer when testing is complete
    writer.close();
    console.log('Kafka producer closed');
}