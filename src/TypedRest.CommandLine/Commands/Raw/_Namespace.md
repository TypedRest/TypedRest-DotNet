---
uid: TypedRest.CommandLine.Commands.Raw
summary: Commands for operating on <xref:TypedRest.Endpoints.Raw>.
---
The counterpart to the blob and upload endpoints. Unlike the other commands these do not read entities from the console; instead, their arguments are paths on the local file system, and the data is streamed straight between the file and the server.

```sh
myapp contacts 1337 avatar download avatar.png
myapp contacts 1337 avatar upload new-avatar.png
myapp contacts 1337 avatar delete
```
