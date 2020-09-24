# ComSim

The TAG *Network Communication Simulator* (or *TAG ComSim*) is a *white-label* console utility application written in C# provided by [Trust Anchor Group](https://trustanchorgroup.com/)
(**TAG** for short). It can be used to simulate network communication traffic in large-scale networks. Features:

* **Extensible**: It can be extended by external modules to provide support for new features, communication protocols, etc.
* **Scriptable**: Logic can be provided through use of [script](https://lab.tagroot.io/Script.md).
* **Models**: Models are defined in XML files, and include support for defining:
	* Actors
	* Probability Distributions
	* Actions
	* Events
	* Statistics
	* Report Output

## License

You should carefully read the following terms and conditions before using this software. Your use of this software indicates your acceptance of this 
license agreement and warranty. If you do not agree with the terms of this license, or if the terms of this license contradict with your local laws, 
you must remove any files from the TAG ComSim from your storage devices and cease to use it. The terms of this license are subjects of changes 
in future versions of the TAG ComSim.

You may not use, copy, emulate, clone, rent, lease, sell, modify, decompile, disassemble, otherwise reverse engineer, or transfer the licensed program, 
or any subset of the licensed program, except as provided for in this agreement. Any such unauthorised use shall result in immediate and automatic 
termination of this license and may result in criminal and/or civil prosecution.

The source code and libraries provided in this repository (including references to external libraries) is provided open and without charge for the following uses:

* For **Personal evaluation**. Personal evaluation means evaluating the code, its libraries and underlying technologies, including learning about underlying technologies.
Redistribution of artefacts or source code requries attribution to the [original source code repository](https://github.com/Trust-Anchor-Group/ComSim), as well as a 
license agreement including provisions equivalent to this license agreement.

* For **Academic use**. This includes research projects, student projects or classroom projects. Redistribution of artefacts or source code requries attribution to the 
[original source code repository](https://github.com/Trust-Anchor-Group/ComSim), as well as a license agreement including provisions equivalent to this license agreement. 
Attribution and reference in published articles is encouraged. If access to other technologies based on IEEE P1451.99 is desired, please [contact Trust Anchor Group AB](#contact).

* For **Security analysis**. If you perform any security analysis on the code, to see what security aspects the code might have, all we request of you, is that you 
maintain the information in a confidential manner, inform us of any findings privately, with sufficient anticipation, before publishing your findings, in accordance 
with *ethical hacking* guidelines. By informing us at least forty-five (45) days before publication of the findings, you provide us with sufficient time to address 
any vulnerabilities you have found. Such contributions are much appreciated and will be acknowledged. (Note that informing us about vulnerabilities in public fora,
such as issues here on GitHub, counts as publishing, and not private.)

* For **Commercial use**. Use of the white-label TAG ComSim for commercial use is permitted. Replication and re-publication of source code is permitted with
attribution to the [original source code repository](https://github.com/Trust-Anchor-Group/ComSim), as well as a license agreement including provisions equivalent 
to this license agreement.

**Note**: All rights to the source code are reserved and exclusively owned by Trust Anchor Group AB. Any contributions made to the TAG ComSim repository 
become the intellectual property of Trust Anchor Group AB.

This software is provided by the copyright holder and contributors "as is" and any express or implied warranties, including, but not limited to, the implied 
warranties of merchantability and fitness for a particular purpose are disclaimed. In no event shall the copyright owner or contributors be liable for any 
direct, indirect, incidental, special, exemplary, or consequential damages (including, but not limited to, procurement of substitute goods or services; loss 
of use, data, or profits; or business interruption) however caused and on any theory of liability, whether in contract, strict liability, or tort (including 
negligence or otherwise) arising in any way out of the use of this software, even if advised of the possibility of such damage.

The TAG ComSim is © Trust Anchor Group AB 2020. All rights reserved.

## Command-line arguments

The *TAG ComSim* console application is run from a command-prompt. Command-line arguments are as follows:

| Argument                | Description |
|:------------------------|:------------|
| `-i FILENAME`           | Specifies the filename of the model to use during simulation. The file must be an XML file that conforms to the http://trustanchorgroup.com/Schema/ComSim.xsd namespace. Schema: [ComSim.xsd](ComSim/Schema/ComSim.xsd) in the repository. |
| `-l LOG_FILENAME`       | Redirects logged events to a log file. |
| `-lt LOG_TRANSFORM`     | File name of optional XSLT transform for use with log file. |
| `-lc`                   | Log events to the console. |
| `-s SNIFFER_FOLDER`     | Optional folder for storing network sniff files. |
| `-st SNIFFER_TRANSFORM` | File name of optional XSLT transform for use with sniffers. |
| `-d APP_DATA_FOLDER`    | Points to the application data folder. Required if storage of data in a local database is necessary for the simulation. (Storage can include generated user credentials so that the same user identities can be used across simulations.) |
| `-e`                    | If encryption is used by the database. Default=no encryption. |
| `-bs BLOCK_SIZE`        | Block size, in bytes. Default=8192. |
| `-bbs BLOB_BLOCK_SIZE`  | BLOB block size, in bytes. Default=8192. |
| `-enc ENCODING`         | Text encoding. Default=UTF-8 |
| `-mr FILENAME`          | Generates a [Markdown](https://lab.tagroot.io/Markdown.md) Report file after simulation. |
| `-xr FILENAME`          | Generates an XML report file after simulation. |
| `-master RELNAME`       | Adds a Master file declaration to the top of markdown reports. The reference must be relative to the generated report file. |
| `-css RELNAME`          | Adds a CSS file declaration to the top of markdown reports. The reference must be relative to the generated report file. |
| `-?`                    | Displays command-line help. |

## Examples

Following is a list of simulation examples that introduces different concenpts in simple examples. They can also be used as the basis for 
new simulation models:

| Example                                                   | Description                                                                                                                                                                                                                                                      | Simulation Report                                              |
|:----------------------------------------------------------|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|:---------------------------------------------------------------|
| [SimpleChatMessages.xml](Examples/SimpleChatMessages.xml) | Simulates a few users using XMPP to send chat messages between each other. Illustrates stochastic and external events.                                                                                                                                           | [Report](https://lab.tagroot.io/Reports/SimpleChatMessages.md) |
| [GuessANumber.xml](Examples/GuessANumber.xml)             | Simulates a few users using XMPP to play the simple game of guessing a number. Demonstrates how to persist states across events, use event guards, custom logging, simplified actions and custom graphs. Builds on the `SimpleChatMessages` example.             | [Report](https://lab.tagroot.io/Reports/GuessANumber.md)       |
| [ServerPerformance.xml](Examples/ServerPerformance.xml)   | Shows how to measure server performance during a simulation. Introduces Performance Counters, Web Services and Timers. Builds on the `GuessANumber` example.                                                                                                     | [Report](https://lab.tagroot.io/Reports/ServerPerformance.md)  |
| [XmlMessages.xml](Examples/XmlMessages.xml)               | Demonstrates how to send, receive and validate custom XML messages between XMPP Clients. Introduces custom messages, message handlers, public key cryptography, [XMLDSIG](https://www.w3.org/TR/xmldsig-core/), local performance counters and parallel actions. | [Report](https://lab.tagroot.io/Reports/XmlMessages.md)        |

## Extending the Simulator

You can extend the simulator by providing links to your own .NET Standard assemblies, and reference them from your simulation model.
To extend the simulator, create classes that implement the [ISimulationNode](TAG.Simulator/ISimulationNode.cs) interface, defined in the 
[TAG.Simulator](TAG.Simulator) library. The classes must implement the public a constructor taking a `ISimulationNode` argument representing
its parent node, and a `Model` parameter, representing the simulation model in which the object is created. To extend the simulator with new 
types of actors (for instance, new communication protocols), these classes must also implement the [IActorNode](TAG.Simulator/IActorNode.cs) 
interface, or derive from the [Actor](TAG.Simulator/ObjectModel/Actors/Actor.cs) class.

Following is a list of extension modules provided in the repository:

| Example                                  | Description                                                  |
|:-----------------------------------------|:-------------------------------------------------------------|
| [TAG.Simulator.XMPP](TAG.Simulator.XMPP) | Defines simulation extensions for actors communicating XMPP. |

## Script extensions

You can extend the script engine using in the simulator, by implementing new functions or custom parsers. See
[Script reference](https://lab.tagroot.io/Script.md) for more information. Any assembly you reference in your simulation model
that contains script extensions, will automatically be used to extend the script engine of the simulator.

The simulator itself provides the following extensions:

| Entity   | Description |
|:---------|:------------|
| `Model`  | A predefined variable pointing to the simulation model being executed. |
| `Global` | A set of global variables. Accessible across events. Variables used in event handlers are accessible only from that event handler. |

## Schema Files

Syntax of simulation nodes are defined in XML Schema files. The following table lists XML Schema files defined by the project:

| Schema                                                     | Namespace                                            | Description                                                                |
|:-----------------------------------------------------------|:-----------------------------------------------------|:---------------------------------------------------------------------------|
| [ComSim.xsd](TAG.Simulator/Schema/ComSim.xsd)              | `http://trustanchorgroup.com/Schema/ComSim.xsd`      | Defines the main structure of a simulation model file.                     |
| [ComSimXmpp.xsd](TAG.Simulator.XMPP/Schema/ComSimXmpp.xsd) | `http://trustanchorgroup.com/Schema/ComSim/XMPP.xsd` | Defines simulation extensions for actors communicating XMPP.               |

## Contact

You can choose to contact us via our [online feedback form](https://lab.tagroot.io/Feedback.md), via [company e-mail](mailto:info@trustanchorgroup.com), or the
[author directly](https://www.linkedin.com/in/peterwaher/).