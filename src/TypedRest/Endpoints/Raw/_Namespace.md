---
uid: TypedRest.Endpoints.Raw
summary: Raw endpoints allow you to transmit binary data rather than serialized objects.
---
Use these when a resource is a file rather than an entity: avatars, attachments, exports, etc. Instead of running the body through a serializer, they hand you a `Stream` (or take one), so large payloads are transferred without being buffered in memory. Extension methods add overloads that take a file path instead. See [Blob endpoint](https://typedrest.net/endpoints/raw/blob/) and [Upload endpoint](https://typedrest.net/endpoints/raw/upload/) for their methods and usage.

An upload endpoint accepts data either as a raw request body or as `multipart/form-data`; pass a form field name to its constructor to pick the latter, which is what HTML file upload forms and most web frameworks expect.

Since a blob's operations often depend on permissions, `ProbeAsync()` asks the server (via HTTP OPTIONS) what is currently allowed and reflects the answer in `DownloadAllowed`, `UploadAllowed` and `DeleteAllowed`.
