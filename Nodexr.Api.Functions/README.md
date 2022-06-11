This project contains an Azure Functions API which is currently unused. The long-term goal is for this API to serve the following two purposes:

- Add the ability to create a "proper" shareable link which retains all information (e.g., specific nodes and their positions). The current "shareable link" feature just uses the regular expression itself, which loses a lot of useful information. This would work similar to the sharing feature of Regex101 or similar tools, and will *not* require user accounts.
- Add an "expression browser" to allow node trees to be published and searched for.