using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using umfgcloud.loja.dominio.service.DTO;

namespace umfgcloud.aplicacao.service.testes.Classes
{
    [TestClass]
    public sealed class ProdutoServicoTestes : AbstractServicoTestes
    {
        private const string C_OWNER = "Juliano Maciel";
        private const string C_CATEGORY = "produto";

        #region AdicionarAsync

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AdicionarAsync_Sucesso()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);
                var dto = GetProdutoRequestDTO();

                // Act
                await servico.AdicionarAsync(dto);

                // Assert
                var produto = (await servico.ObterTodosAsync()).FirstOrDefault();

                Assert.IsNotNull(produto);
                Assert.AreNotEqual(Guid.Empty, produto.Id);
                Assert.AreEqual("TESTE", produto.Descricao);
                Assert.AreEqual("123456789", produto.EAN);
                Assert.AreEqual(39.90m, produto.ValorCompra);
                Assert.AreEqual(89.90m, produto.ValorVenda);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AdicionarAsync_FalhaValorCompraNegativo()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);
                var dto = GetProdutoRequestDTO(valorCompra: -39.90m);

                // Act / Assert
                await Assert.ThrowsExceptionAsync<InvalidDataException>(() => servico.AdicionarAsync(dto));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AdicionarAsync_FalhaValorVendaNegativo()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);
                var dto = GetProdutoRequestDTO(valorVenda: -89.90m);

                // Act / Assert
                await Assert.ThrowsExceptionAsync<InvalidDataException>(() => servico.AdicionarAsync(dto));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion AdicionarAsync

        #region ObterTodosAsync

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_ObterTodosAsync_SucessoListaVazia()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                // Act
                var produtos = await servico.ObterTodosAsync();

                // Assert
                Assert.IsNotNull(produtos);
                Assert.AreEqual(0, produtos.Count());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_ObterTodosAsync_Sucesso()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                await servico.AdicionarAsync(GetProdutoRequestDTO(descricao: "PRODUTO 1", ean: "111111111"));
                await servico.AdicionarAsync(GetProdutoRequestDTO(descricao: "PRODUTO 2", ean: "222222222"));

                // Act
                var produtos = (await servico.ObterTodosAsync()).ToList();

                // Assert
                Assert.AreEqual(2, produtos.Count);
                Assert.IsTrue(produtos.Any(x => x.Descricao == "PRODUTO 1" && x.EAN == "111111111"));
                Assert.IsTrue(produtos.Any(x => x.Descricao == "PRODUTO 2" && x.EAN == "222222222"));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion ObterTodosAsync

        #region ObterPorIdAsync

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_ObterPorIdAsync_Sucesso()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);
                var dto = GetProdutoRequestDTO();

                await servico.AdicionarAsync(dto);
                var id = (await servico.ObterTodosAsync()).First().Id;

                // Act
                var produto = await servico.ObterPorIdAsync(id);

                // Assert
                Assert.IsNotNull(produto);
                Assert.AreEqual(id, produto.Id);
                Assert.AreEqual("TESTE", produto.Descricao);
                Assert.AreEqual("123456789", produto.EAN);
                Assert.AreEqual(39.90m, produto.ValorCompra);
                Assert.AreEqual(89.90m, produto.ValorVenda);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_ObterPorIdAsync_FalhaRegistroNaoEncontrado()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                // Act / Assert
                await Assert.ThrowsExceptionAsync<ApplicationException>(() => servico.ObterPorIdAsync(Guid.NewGuid()));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion ObterPorIdAsync

        #region AtualizarAsync

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AtualizarAsync_Sucesso()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                await servico.AdicionarAsync(GetProdutoRequestDTO());
                var id = (await servico.ObterTodosAsync()).First().Id;

                var dtoAtualizacao = new ProdutoDTO.ProdutoRequestWithId()
                {
                    Id = id,
                    Descricao = "ATUALIZADO",
                    EAN = "987654321",
                    ValorCompra = 49.90m,
                    ValorVenda = 99.90m,
                };

                // Act
                await servico.AtualizarAsync(dtoAtualizacao);

                // Assert
                var produto = await servico.ObterPorIdAsync(id);

                Assert.AreEqual("ATUALIZADO", produto.Descricao);
                Assert.AreEqual("987654321", produto.EAN);
                Assert.AreEqual(49.90m, produto.ValorCompra);
                Assert.AreEqual(99.90m, produto.ValorVenda);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AtualizarAsync_FalhaValorCompraNegativo()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                await servico.AdicionarAsync(GetProdutoRequestDTO());
                var id = (await servico.ObterTodosAsync()).First().Id;

                var dtoAtualizacao = new ProdutoDTO.ProdutoRequestWithId()
                {
                    Id = id,
                    Descricao = "ATUALIZADO",
                    EAN = "987654321",
                    ValorCompra = -49.90m,
                    ValorVenda = 99.90m,
                };

                // Act / Assert
                await Assert.ThrowsExceptionAsync<InvalidDataException>(() => servico.AtualizarAsync(dtoAtualizacao));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AtualizarAsync_FalhaValorVendaNegativo()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                await servico.AdicionarAsync(GetProdutoRequestDTO());
                var id = (await servico.ObterTodosAsync()).First().Id;

                var dtoAtualizacao = new ProdutoDTO.ProdutoRequestWithId()
                {
                    Id = id,
                    Descricao = "ATUALIZADO",
                    EAN = "987654321",
                    ValorCompra = 49.90m,
                    ValorVenda = -99.90m,
                };

                // Act / Assert
                await Assert.ThrowsExceptionAsync<InvalidDataException>(() => servico.AtualizarAsync(dtoAtualizacao));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AtualizarAsync_FalhaRegistroNaoEncontrado()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                var dtoAtualizacao = new ProdutoDTO.ProdutoRequestWithId()
                {
                    Id = Guid.NewGuid(),
                    Descricao = "ATUALIZADO",
                    EAN = "987654321",
                    ValorCompra = 49.90m,
                    ValorVenda = 99.90m,
                };

                // Act / Assert
                await Assert.ThrowsExceptionAsync<ApplicationException>(() => servico.AtualizarAsync(dtoAtualizacao));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion AtualizarAsync

        #region RemoverAsync

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_RemoverAsync_Sucesso()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                await servico.AdicionarAsync(GetProdutoRequestDTO());
                var id = (await servico.ObterTodosAsync()).First().Id;

                // Act
                await servico.RemoverAsync(id);

                // Assert
                var produtos = await servico.ObterTodosAsync();
                Assert.AreEqual(0, produtos.Count());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_RemoverAsync_FalhaRegistroNaoEncontrado()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                // Act / Assert
                await Assert.ThrowsExceptionAsync<ApplicationException>(() => servico.RemoverAsync(Guid.NewGuid()));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion RemoverAsync

        #region Construtor

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public void ProdutoServico_Instanciar_Falha()
        {
            try
            {
                // Arrange
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());

                // Act / Assert
                Assert.ThrowsException<InvalidDataException>(() => GetProdutoServicoInvalidJWT(context));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion Construtor

        #region Helpers

        private static ProdutoDTO.ProdutoRequest GetProdutoRequestDTO(
            string descricao = "TESTE",
            string ean = "123456789",
            decimal valorCompra = 39.90m,
            decimal valorVenda = 89.90m)
            => new()
            {
                Descricao = descricao,
                EAN = ean,
                ValorCompra = valorCompra,
                ValorVenda = valorVenda,
            };

        #endregion Helpers
    }
}
