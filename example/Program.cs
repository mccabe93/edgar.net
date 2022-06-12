using Examples;

Console.WriteLine("Call some example functions . . . ");

//CompanyManagerExamples exmaples = new CompanyManagerExamples();

//await exmaples.GetAllCompanyInfo();

FormManagerExamples examples = new FormManagerExamples();

await examples.GetFormDataFeed();
await examples.GetFormData(0);