{
	"cpuPercent":PerformanceCounters.GetCounter("Processor","_Total","% Processor Time").NextValue(),
	"networkBytesPerSec":PerformanceCounters.GetCounter("Network Interface","Microsoft Hyper-V Network Adapter","Bytes Total/sec").NextValue(),
	"memoryMBytesFree":PerformanceCounters.GetCounter("Memory","Available MBytes").NextValue(),
	"xmppCpu":PerformanceCounters.GetCounter("Process","Waher.IoTGateway.Svc","% Processor Time").NextValue(),
	"mqttCpu":PerformanceCounters.GetCounter("Process","mosquitto","% Processor Time").NextValue(),
	"mqCpu":PerformanceCounters.GetCounter("Process","amqrmppa","% Processor Time").NextValue(),
	"xmppMemory":PerformanceCounters.GetCounter("Process","Waher.IoTGateway.Svc","Working Set - Private").NextValue(),
	"mqttMemory":PerformanceCounters.GetCounter("Process","mosquitto","Working Set - Private").NextValue(),
	"mqMemory":PerformanceCounters.GetCounter("Process","amqrmppa","Working Set - Private").NextValue(),
	"xmppIo":PerformanceCounters.GetCounter("Process","Waher.IoTGateway.Svc","IO Other Bytes/sec").NextValue(),
	"mqttIo":PerformanceCounters.GetCounter("Process","mosquitto","IO Other Bytes/sec").NextValue(),
	"mqIo":PerformanceCounters.GetCounter("Process","amqrmppa","IO Other Bytes/sec").NextValue()
}