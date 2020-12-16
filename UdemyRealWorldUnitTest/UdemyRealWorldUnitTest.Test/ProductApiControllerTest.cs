using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using UdemyRealWorldUnitTest.Web;
using UdemyRealWorldUnitTest.Web.Controllers;
using UdemyRealWorldUnitTest.Web.Repository;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductApiControllerTest
    {
        // IRepository taklit ediyorum. Mock ile.
        private readonly Mock<IRepository<Product>> _mockRepo;

        // Asıl Test edeçeğim nesne
        private readonly ProductsApiController _productsApiController;

        private List<Product> products;

        public ProductApiControllerTest()
        {
            // MockBehavior.Strict ilgili dependency 'lerin hepsini Mock lamak için: 
            // MockBehavior.Loose gevşek, ilgili dependency 'yi illede yazmama gerek yok.
            _mockRepo = new Mock<IRepository<Product>>(MockBehavior.Loose);
            _productsApiController = new ProductsApiController(_mockRepo.Object);

            products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Kalem",
                    Price = 100,
                    Stock = 50,
                    Color = "Kırmızı"
                },
                 new Product
                {
                    Id = 2,
                    Name = "Defter",
                    Price = 200,
                    Stock = 500,
                    Color = "Mavi"
                }
            };
        }

        #region GetProduct()
        [Fact]
        public async void GetProduct_ActionExecutes_ReturnOkResultWithProduct()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(products); // mockrepo ile Repository 'yi mocklaryıp geriye products dönmesini simule ediyoruz.

            var result = await _productsApiController.GetProduct();

            // test 1 Ok result geliyor mu?
            var okResult = Assert.IsType<OkObjectResult>(result);

            // test 2 dönen Ok içerisindeki tip IEnumerable<Product> mi?
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            // test 3 dönen değer sayısı 2 olmalı?
            Assert.Equal<int>(2, returnProducts.ToList().Count);

        }
        #endregion GetProduct()

        #region GetProduct(int id)
        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _productsApiController.GetProduct(productId);

            // test 1 NotFoundResult result geliyor mu?
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_IdValid_ReturnOkResult(int productId)
        {
            Product product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _productsApiController.GetProduct(productId);

            // test 1 OkObjectResult result geliyor mu?
            var okResult = Assert.IsType<OkObjectResult>(result);

            // test 2 okResult.Value Product tipinde mi?
            var returnProduct = Assert.IsType<Product>(okResult.Value);

            //test 3 returnProduct 'in Propert değerleri ile product 'in property değerleri aynı mı?
            Assert.Equal(productId, returnProduct.Id);
            Assert.Equal(product.Name, returnProduct.Name);
        }
        #endregion GetProduct(int id)

        #region     PutProduct(int id, Product product)

        public void PutProduct_IdIsNotEqualProduct?

        #endregion  PutProduct(int id, Product product)
    }
}
