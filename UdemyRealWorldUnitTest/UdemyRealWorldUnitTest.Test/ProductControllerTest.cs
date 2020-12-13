using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UdemyRealWorldUnitTest.Web;
using UdemyRealWorldUnitTest.Web.Controllers;
using UdemyRealWorldUnitTest.Web.Repository;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{


    public class ProductControllerTest
    {
        // IRepository taklit ediyorum. Mock ile.
        private readonly Mock<IRepository<Product>> _mockRepo;

        // Asıl Test edeçeğim nesne
        private readonly ProductsController _productsController;

        private List<Product> products;

        public ProductControllerTest()
        {
            // MockBehavior.Strict ilgili dependency 'lerin hepsini Mock lamak için: 
            // MockBehavior.Loose gevşek, ilgili dependency 'yi illede yazmama gerek yok.
            _mockRepo = new Mock<IRepository<Product>>(MockBehavior.Loose);
            _productsController = new ProductsController(_mockRepo.Object);

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

        #region Index Testleri

        // Index View Testleri 
        // 1. Geriye Mutlaka bir View dönmeli.
        // 2. Product Listesi dönmeli.


        /// <summary>
        /// Dönen değerin tipi ViewResult 'mi
        /// Products boş
        /// </summary>
        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            // Geri dönüş değeri tipi ViewResult 'mi???
            var result = await _productsController.Index();
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            /// mock ile products dönmesini simule ettik, ya da taklit ettik.
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(products);

            // 1. Kontrol geriye ViewResult dönüyor mu?
            var result = await _productsController.Index();
            var viewResult = Assert.IsType<ViewResult>(result);

            // 2. viewResult.Model bir productList türünde mi?
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            // 3. Dönen productlist 'in sayısı 2 'mi.
            Assert.Equal<int>(2, productList.Count());
        }
        #endregion Index Testleri

        #region Detail Testleri

        /// <summary>
        /// id null testi
        /// </summary>
        [Fact] // Parametre almadığı için Fact
        public async void Detail_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        /// <summary>
        /// NotFound dönme testi
        /// </summary>
        [Fact] // Parametre almadığı için Fact
        public async void Detail_IdInvalid_ReturnNotFound()
        {
            Product product = null;
            /// mock ile Details içerisindeki: var product = await _repository.GetById(id.Value); simule ediyoruz.
            _mockRepo.Setup(repo => repo.GetById(0)).ReturnsAsync(product);

            var result = await _productsController.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        /// <summary>
        ///  Id düzgün geriye Product dönüyor mu?
        /// </summary>
        [Theory] // Parametre aldığı için Theory
        [InlineData(1)]
        [InlineData(2)]

        public async void Detail_ValidId_ReturnProduct(int productId)
        {
            Product product = products.First(x => x.Id == productId);

            /// mock ile Details içerisindeki: var product = await _repository.GetById(id.Value); simule ediyoruz.
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productsController.Details(productId);

            // 1. Test. Tipi ViewResult mi?
            var viewResult = Assert.IsType<ViewResult>(result);

            // 2. Test. Model in tipi Product mi?
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            // 3. Test. Dönen Model 'in Property 'lerini de test ediyoruz.
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }


        #endregion Detail Testleri

        #region Create Testleri

        /// <summary>
        ///  Create Get metodunun testi
        ///  Dönen değer ViewResult mi bakıyoruz sadece.
        /// </summary>
        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _productsController.Create();
            Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// 40. Create[POST] methodunun test edilmesi-2
        /// Post metodunun Invalid Model testi
        /// </summary>
        [Fact]
        public async void CreatePOST_InValidModelState_ReturnView()
        {
            _productsController.ModelState.AddModelError("Name", "Name alanı gereklidir");

            var result = await _productsController.Create(products.First());

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }


        /// <summary>
        /// 41. Create[POST] methodunun test edilmesi-3
        /// Post metodunun Model Valid ise Index 'e yönleniyor mu? 
        /// </summary>
        [Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Create(products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        //42. Create[POST] methodunun test edilmesi-4
        // Post metodunda Model Valid ise Product Create oluyor mu?
        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;

            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>()))
                    .Callback<Product>(x => newProduct = x);

            var result = await _productsController.Create(products.First());

            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()),
                Times.Once);

            Assert.Equal(products.First().Id, newProduct.Id);
        }


        // 43. Create[POST] methodunun test edilmesi-5
        // Post Create 'e ModelState 'i geçerli olmayan bir Product gönderilince repository.Create() metodunun çalışmaması lazım.
        [Fact]
        public async void CreatePOST_InValidModelState_NeverCreateExecute()
        {
            _productsController.ModelState.AddModelError("Name", "Hata var gardaş!!!");

            var result = await _productsController.Create(products.First());
            //
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);
        }

        #endregion Create Testleri

        #region Edit Testleri

        #region Edit Get metod testleri
        // 44. Edit methodunun test edilmesi-1
        // Edit get metodunun test edilmesi.  id null gönderince
        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Edit(null);

            // test 1 dönen tip RedirectToActionResult mi?
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            // test 2 Index 'emi redirect oldu?
            Assert.Equal("Index", redirect.ActionName);

        }

        // 45. Edit methodunun test edilmesi-2
        // Edit get metodunun test edilmesi.  product == null ise NotFound dönmesi lazım.
        [Theory]
        [InlineData(3)] // olmayan data veriyoruz
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;
            // ProductsController 'daki Edit get metodunun içerisindeki GetById mock-kandırıyoruz. product=null dönmüş gibi
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            // Metodu çağırıyorum
            var result = await _productsController.Edit(productId);

            // test 1. dönen tip NotFoundResult 'mi?
            var redirect = Assert.IsType<NotFoundResult>(result);

            // test 2. statuscode 404 mu? Bunu yazmaya gerek yok!!! NotFoundResult 404 demek zaten.
            Assert.Equal(404, redirect.StatusCode);
        }

        // 46. Edit methodunun test edilmesi-3
        // Edit get metodunun test edilmesi.  product dolu iken return View(product) dönmesi lazım.
        [Theory]
        [InlineData(2)] // 
        public async void Edit_ActionExecutes_ReturnProduct(int productId)
        {
            Product product = products.First(x => x.Id == productId);
            // ProductsController 'daki Edit get metodunun içerisindeki GetById mock-kandırıyoruz. product getById ile bulunmuş gibi dönmüş gibi
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            // Metodu çağırıyorum
            var result = await _productsController.Edit(productId);

            // test 1. dönen tip ViewResult 'mi?
            var viewResult = Assert.IsType<ViewResult>(result);

            // test 2.  viewResult Model tipi Product mi?
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            // test 3. dönen resultProduct 'in değerlerini karşılaştırıyoruz.
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);

        }
        #endregion Edit Get metod testleri


        #region Edit Post Testleri

        /// <summary>
        /// 47. Edit[POST] methodunun test edilmesi-4
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public void EditPOST_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {

            var result = _productsController.Edit(2, products.First(x => x.Id == productId));

            var redirect = Assert.IsType<NotFoundResult>(result);

            // 404 testine gerek yok.
        }


        /// <summary>
        /// 48. Edit[POST] methodunun test edilmesi-5
        /// ModelState IsValid değilse testi
        /// Geriye return View(product) dönmeli.
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public void EditPOST_InvalidModelState_ReturnView(int productId)
        {
            // ModelState 'e bir hata ekliyorum.
            _productsController.ModelState.AddModelError("Name", "");

            var result = _productsController.Edit(1, products.First(x => x.Id == productId));

            // test1 dönen tip viewresult mi?
            var viewResult = Assert.IsType<ViewResult>(result);

            // test2 dönen viewresult in içerisindeli Model in tipi Product mi?
            Assert.IsType<Product>(viewResult.Model);
        }

        /// <summary>
        /// 49. Edit[POST] methodunun test edilmesi-6
        /// ModelState Valid ise Index sayfasına redirect oluyor mu testi?
        /// _mock yapmıycaz. Update işlemini test etmiyorum sadece Update'ten sonra Index 'e redirect oluyor mu?
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModel_ReturnRedirectToIndexAction(int productId)
        {
            var result = _productsController.Edit(productId, products.First(x => x.Id == productId));

            // Modelstate Valid ise, Index 'e redirect oluyor mu?
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }


        /// <summary>
        /// 50. Edit[POST] methodunun test edilmesi-7
        /// ModelState Valid ise Update işlemi execute ediliyor mu?
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModel_UpdateMethodExecute(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.Update(product));

            var result = _productsController.Edit(productId, product);

            // Update metodu bir kez çalıştı mı?
            _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
        }


        #endregion Edit Post Testleri

        #endregion Edit Testleri


        #region Delete Testleri

        #region Delete Get Testleri

        /// <summary>
        /// 51. Delete methodunun test edilmesi-1
        /// </summary>
        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _productsController.Delete(null);

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// 52. Delete methodunun test edilmesi-2
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _productsController.Delete(productId);

            var redirectResult = Assert.IsType<NotFoundResult>(result);

            // 404 dönüyor mu kontrol etmeye gerek yok
            Console.WriteLine(redirectResult.StatusCode);
        }

        /// <summary>
        /// 53. Delete methodunun test edilmesi-3
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            Product product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _productsController.Delete(productId);  

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }

        #endregion Delete Get Testleri

        #region DeleteConfirmed POST Testleri

        /// <summary>
        /// 54. Delete methodunun test edilmesi-4
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _productsController.DeleteConfirmed(productId);

            Assert.IsType<RedirectToActionResult>(result);
        }

        /// <summary>
        /// 55. Delete methodunun test edilmesi-5
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.Delete(product));

            await _productsController.DeleteConfirmed(productId);

            _mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
        }

        #endregion DeleteConfirmed POST Testleri

        #endregion Delete Testleri

        #region ProductExists Testleri
        /// <summary>
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(0)]
        public void ProductExists_ProductIsNull_ReturnFalse(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

             var result = _productsController.ProductExists(productId);

            Assert.False(result);
        }

        /// <summary>
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public void ProductExists_ProductIsNotNull_ReturnTrue(int productId)
        {
            Product product = products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result =  _productsController.ProductExists(productId);

            Assert.True(result);
        }

        #endregion ProductExists Testleri
    }
}