using NUnit.Framework;
using TechTalk.SpecFlow;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using ProductApi.Models;

namespace ProductApi.BDDTests.Steps
{
  [Binding]
  public class ProductSteps
  {
    private readonly HttpClient _client;
    private Product _responseProduct;
    private HttpResponseMessage _response;
    private string _baseUrl = "https://localhost:5001/api/products"; // adjust to your API base URL

    public ProductSteps()
    {
      _client = new HttpClient();
    }

    [Given(@"the product does not already exist")]
    public async Task GivenTheProductDoesNotAlreadyExist()
    {
      _response = await _client.GetAsync($"{_baseUrl}?name=Laptop");
      if (_response.IsSuccessStatusCode)
      {
        var products = JsonConvert.DeserializeObject<List<Product>>(await _response.Content.ReadAsStringAsync());
        foreach (var product in products)
        {
          await _client.DeleteAsync($"{_baseUrl}/{product.Id}");
        }
      }
    }

    [When(@"I add a new product with name ""(.*)"", price ""(.*)"", and stock ""(.*)""")]
    public async Task WhenIAddANewProductWithNamePriceAndStock(string name, decimal price, int stock)
    {
      var product = new Product { Name = name, Price = price, Stock = stock };
      _response = await _client.PostAsJsonAsync(_baseUrl, product);
    }

    [Then(@"the product should be added successfully")]
    public void ThenTheProductShouldBeAddedSuccessfully()
    {
      Assert.AreEqual(HttpStatusCode.Created, _response.StatusCode);
    }

    [Then(@"I should see the product with name ""(.*)"" in the list of products")]
    public async Task ThenIShouldSeeTheProductWithNameInTheListOfProducts(string name)
    {
      _response = await _client.GetAsync(_baseUrl);
      var products = JsonConvert.DeserializeObject<List<Product>>(await _response.Content.ReadAsStringAsync());
      Assert.IsTrue(products.Exists(p => p.Name == name));
    }

    [Given(@"a product with name ""(.*)"" exists")]
    public async Task GivenAProductWithNameExists(string name)
    {
      await WhenIAddANewProductWithNamePriceAndStock(name, 999.99M, 10);
    }

    [When(@"I retrieve the product by its ID")]
    public async Task WhenIRetrieveTheProductByItsID()
    {
      var products = JsonConvert.DeserializeObject<List<Product>>(await _client.GetStringAsync(_baseUrl));
      _responseProduct = products.Find(p => p.Name == "Laptop");
      _response = await _client.GetAsync($"{_baseUrl}/{_responseProduct.Id}");
    }

    [Then(@"the product details should show name ""(.*)"", price ""(.*)"", and stock ""(.*)""")]
    public async Task ThenTheProductDetailsShouldShowNamePriceAndStock(string name, decimal price, int stock)
    {
      var product = JsonConvert.DeserializeObject<Product>(await _response.Content.ReadAsStringAsync());
      Assert.AreEqual(name, product.Name);
      Assert.AreEqual(price, product.Price);
      Assert.AreEqual(stock, product.Stock);
    }

    [When(@"I update the product to have name ""(.*)"" and price ""(.*)""")]
    public async Task WhenIUpdateTheProductToHaveNameAndPrice(string name, decimal price)
    {
      _responseProduct.Name = name;
      _responseProduct.Price = price;
      _response = await _client.PutAsJsonAsync($"{_baseUrl}/{_responseProduct.Id}", _responseProduct);
    }

    [Then(@"the product details should be updated to name ""(.*)"" and price ""(.*)""")]
    public async Task ThenTheProductDetailsShouldBeUpdatedToNameAndPrice(string name, decimal price)
    {
      var product = JsonConvert.DeserializeObject<Product>(await _client.GetStringAsync($"{_baseUrl}/{_responseProduct.Id}"));
      Assert.AreEqual(name, product.Name);
      Assert.AreEqual(price, product.Price);
    }

    [When(@"I delete the product by its ID")]
    public async Task WhenIDeleteTheProductByItsID()
    {
      _response = await _client.DeleteAsync($"{_baseUrl}/{_responseProduct.Id}");
    }

    [Then(@"the product should no longer exist in the system")]
    public async Task ThenTheProductShouldNoLongerExistInTheSystem()
    {
      var response = await _client.GetAsync($"{_baseUrl}/{_responseProduct.Id}");
      Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
  }
}
