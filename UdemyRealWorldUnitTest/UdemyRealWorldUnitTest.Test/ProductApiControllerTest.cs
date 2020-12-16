using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.Web;
using UdemyRealWorldUnitTest.Web.Controllers;
using UdemyRealWorldUnitTest.Web.Helper;
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


        private readonly Helpers _helper;
        private List<Product> products;

        public ProductApiControllerTest()
        {
            // MockBehavior.Strict ilgili dependency 'lerin hepsini Mock lamak için: 
            // MockBehavior.Loose gevşek, ilgili dependency 'yi illede yazmama gerek yok.
            _mockRepo = new Mock<IRepository<Product>>(MockBehavior.Loose);
            _productsApiController = new ProductsApiController(_mockRepo.Object);
            _helper = new Helpers();

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

        /// <summary>
        /// 61. PutProduct methodunun test edilmesi-1
        /// if (id != product.Id)
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public void PutProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = products.First(x => x.Id == productId);

            var result = _productsApiController.PutProduct(2, product);

            var badRquestResult = Assert.IsType<BadRequestResult>(result);

            Assert.Equal(400, badRquestResult.StatusCode);
        }


        /// <summary>
        /// 62. PutProduct methodunun test edilmesi-1
        /// 
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecute_ReturnNoContent(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.Update(product));

            var result = _productsApiController.PutProduct(productId, product);

            // mockRepo Update metodu 1 kere çalışmış mı?
            _mockRepo.Verify(x => x.Update(product), Times.Once);

            var noContentResult = Assert.IsType<NoContentResult>(result);

            Assert.Equal(204, noContentResult.StatusCode);
        }

        #endregion  PutProduct(int id, Product product)

        #region      PostProduct(Product product)


        /// <summary>
        /// 63. PostProduct methodunun test edilmesi
        /// </summary>
        /// <param name="productId"></param>
        [Fact]
        public async void PostProduct_ActionExecute_ReturnCreatedAtAction()
        {
            var product = products.First();

            _mockRepo.Setup(x => x.Create(product)).Returns(Task.CompletedTask);

            var result = await _productsApiController.PostProduct(product);

            // mockRepo Create metodu 1 kere çalışmış mı?
            _mockRepo.Verify(x => x.Create(product), Times.Once);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            // StatusCode testi
            Assert.Equal(201, createdAtActionResult.StatusCode);
            // ActionName testi
            Assert.Equal("GetProduct", createdAtActionResult.ActionName);

        }

        #endregion      PostProduct(Product product)

        #region         DeleteProduct(int id)

        /// <summary>
        ///  64. DeleteProduct methodunun test edilmesi-1
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            // geriye sınıf dönüyor. O Yüzden ActionResult 'ın result ı üzerinden karşılaştırma yapıyorum.
            var resultNotFound = await _productsApiController.DeleteProduct(productId);

            Assert.IsType<NotFoundResult>(resultNotFound.Result);
        }

        /// <summary>
        ///  65. DeleteProduct methodunun test edilmesi-2
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_ActionExecute_ReturnNoContent(int productId)
        {
            Product product = products.First(x => x.Id == productId);

            // 1. mock Product GetById için
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            // 2. mock Delete için
            _mockRepo.Setup(x => x.Delete(product));

            var noContentResult = await _productsApiController.DeleteProduct(productId);

            // mockRepo Create metodu 1 kere çalışmış mı?
            _mockRepo.Verify(x => x.Delete(product), Times.Once);

            Assert.IsType<NoContentResult>(noContentResult.Result);
        }

        #endregion         DeleteProduct(int id)

        #region Business Testi
        /// <summary>
        /// 66. business method test etme
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="total"></param>
        [Theory]
        [InlineData(4, 5, 9)]
        public void Add_SampleValues_ReturnTotal(int a, int b, int total)
        {
            var result = _helper.Add(a, b);

            Assert.Equal(total, result);
        }

        #endregion Business Testi

    }
}
