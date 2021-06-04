{
	"cpuPercent":PerformanceCounterValue("Processor","_Total","% Processor Time"),
	"networkBytesPerSec":PerformanceCounterValue("Network Interface","Microsoft Hyper-V Network Adapter","Bytes Total/sec"),
	"memoryMBytesFree":PerformanceCounterValue("Memory","Available MBytes"),
	"xmppCpu":PerformanceCounterValue("Process","Waher.IoTGateway.Svc","% Processor Time"),
	"mqttCpu":PerformanceCounterValue("Process","mosquitto","% Processor Time"),
	"mqCpu":PerformanceCounterValue("Process","amqrmppa","% Processor Time"),
	"xmppMemory":PerformanceCounterValue("Process","Waher.IoTGateway.Svc","Working Set - Private"),
	"mqttMemory":PerformanceCounterValue("Process","mosquitto","Working Set - Private"),
	"mqMemory":PerformanceCounterValue("Process","amqrmppa","Working Set - Private"),
	"xmppIo":PerformanceCounterValue("Process","Waher.IoTGateway.Svc","IO Other Bytes/sec"),
	"mqttIo":PerformanceCounterValue("Process","mosquitto","IO Other Bytes/sec"),
	"mqIo":PerformanceCounterValue("Process","amqrmppa","IO Other Bytes/sec"),
	"xmppThreads":PerformanceCounterValue("Process","Waher.IoTGateway.Svc","Thread Count"),
	"mqttThreads":PerformanceCounterValue("Process","mosquitto","Thread Count"),
	"mqThreads":PerformanceCounterValue("Process","amqrmppa","Thread Count")
}