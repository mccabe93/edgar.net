using Edgar.Net;
using Examples;

var client = new EdgarClient { CacheResults = true };
await client.InitializeAsync();

var examples = new FormManagerExamples(client);
await examples.DEFM14ExampleAsync();
