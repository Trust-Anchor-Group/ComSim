TODO
========

Features
-------------

[x] Events
	[x] Stochastic events
	[x] External events
	[x] Trigger activities
	[x] Guards
	[x] Clocks
[x] Meta-data
	[x] Title
	[x] Description
	[x] Script
	[x] Section descriptions
[ ] Activities
	[x] Populations
	[x] Execution
	[ ] Activity node with external event conditions, or stochastic conditions
		[ ] Wait element (cf. Conditional)
		[ ] Trigger event
		[ ] Timeout (cf. Otherwise)
		[ ] Call activity
	[x] Counters
	[x] Stat activity node: Increments counters
	[ ]	Sensor data (local/remote)
	[x] Measurements
	[x] Performance counters
	[x] CPU/Memory
	[x] Global variables
	[x] Timing of, and between events/activities. (communication roundtrip)
	[x] Actor variables
[ ] Simulation output
	[x] Statistics
		[x] Activities
		[x] Actions
		[x] Events
		[x] Counters
		[x] Histograms
		[x] Mean, stddev, var, min, max
		[x] Errors, Exceptions (from Log)
		[x] Event type histogram over time
		[x] Custom SQL
	[x] Charts
		[x] History charts
		[x] Use case charts
		[x] Distribution (expected) charts
		[x] Custom comparison graphs (between counters/samples/etc)
	[x] Data as XML
	[x] Report as Markdown
		[x] Distributions
		[x] Models
		[x] Activity charts
		[x] Statistics
		[x] Custom graphs
		[x] Event log statistics
[ ] Distributions
	[ ] Script-based PDF and CDF
	[ ] Arcsine distribution
	[ ] Beta distribution
	[ ] Chi distribution
	[ ] Chi-squared distribution
	[ ] Exponential distribution
	[ ] Gamma distribution
	[ ] Student-t distribution
[x] Generate action documentation (xml, xslt) automatically
[ ] XMPP
	[x] Message handlers
	[x] Iq handlers
	[x] Presence handlers
	[ ] Extension libraries
	[ ] UDP
	[ ] Iq action node
	[x] Message action node
	[ ] Presence action node
[ ] HTTP
	[x] GET
	[x] POST
		[x] String
		[x] JSON
		[x] Xml
		[x] Signed XML
	[ ] Web Service (SOAP)
[x] MQTT
	[x] PUBLISH
	[x] SUBSCRIBE

Examples
--------------

[x] Simple chat
[x] Guess a number
[x] Web Server Load (HTTP GET)
[x] Timing of communication roundtrip
[x] Parallel threads in activity
[ ] Protocols comparison
[ ] Federation
[ ] External events to determine activity node transitions

Documentation
--------------------

[ ] Structure of a simulation file
[ ] TimeBase, TimeUnit, TimeCycle, Duration (cyclic time)
[ ] Explain keys

