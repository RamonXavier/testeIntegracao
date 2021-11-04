using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoBogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using testeIntegracao.Configurations.AutoBogos;
using testesIntegracao;
using testesIntegracao.context;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable CommentTypo

namespace testeIntegracao.Integrations.Controllers
{
    [CollectionDefinition("CursosCollection")]
    public class CursosControllerTests : IClassFixture<WebApplicationFactory<Startup>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;
        private Curso _curso;
        //private readonly HttpClient _httpClient;

        public CursosControllerTests(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            //_httpClient = _factory.CreateClient();
        }

        /*
         *
         * PADROES DE ESCRITA DE TESTE - PADRAO 1 - ESCRITA DOS NOMES DOS TESTES
         *
         * WhenGivenThen
         * QuandoDadosResultados
         * Ex: public void Autenticar_InformarUsuarioESenha_ReceberTokenEStatusCode201
         *
         */

        /*
         *
         * PADROES DE ESCRITA DE TESTE - PADRAO 2 - ETAPAS DOS TESTES
         * 
         * AAA
         *
         * - Arrange (dados de entrada / setup)
         * - Act (requisição / integração / diretório / método)
         * - Assert (resultado)
         */

        [Fact]
        public async Task Buscar_SemDados_RetornaListaDecursosCadastrados()
        {
            // Act
            var _httpClient = _factory.CreateClient();
            var httpClient = await _httpClient.GetAsync("api/Cursos/Buscar");

            var listaCursos = JsonConvert.DeserializeObject<IEnumerable<Curso>>(httpClient.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            // Assert
            Assert.Equal(HttpStatusCode.OK, HttpStatusCode.OK);
            Assert.NotNull(listaCursos);
            foreach (var curso in listaCursos) {_output.WriteLine(curso.DescricaoCurso + " - " + curso.IdCurso + "\n"); }
        }


        [Fact]
        public async Task Criar_InformandoNome_RetornarStatusCode200EObjetoCriadoAsync()
        {
            // PADRÃO 2

            // Arrange
            _curso = new AutoFaker<Curso>(AutoBogosConfiguration.Location).RuleFor(curso => curso.DescricaoCurso, fake => fake.Commerce.ProductName());
            _curso.IdCurso = 0;

            StringContent content = new StringContent(JsonConvert.SerializeObject(_curso), Encoding.UTF8, "application/json");
            // Act
            var httpClient = _factory.CreateClient();
            var httpClientResult = await httpClient.PostAsync("api/Cursos/Criar", content);

            var cursoCriado = JsonConvert.DeserializeObject<Curso>(httpClientResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            if (cursoCriado != null)
            {
                _curso.DescricaoCurso = cursoCriado.DescricaoCurso;
                _curso.IdCurso = cursoCriado.IdCurso;
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpClientResult.StatusCode);
            Assert.NotNull(cursoCriado);
            _output.WriteLine(cursoCriado.DescricaoCurso);
            _output.WriteLine(cursoCriado.IdCurso.ToString());
        }

        [Fact]
        public async Task Atualizar_InformandoIdNome_RetornarStatusCode200EObjetoCriadoAsync()
        {
            // Arrange - Estrutura/Objeto para enviar
            _curso = new Curso()
            {
                IdCurso = 1003,
                DescricaoCurso = "Editar curso"+"_"+DateTime.Now.TimeOfDay
            };
            var content = new StringContent(JsonConvert.SerializeObject(_curso),Encoding.UTF8,"application/json");

            // Act - requisição e mapeamento
            var httpClient = _factory.CreateClient();
            var httpResult = await httpClient.PutAsync($"api/Cursos/Editar/{_curso.IdCurso}", content);
            var cursoAtualizado = JsonConvert.DeserializeObject(httpResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            // Assert - Verificação de retorno/Conclusão
            Assert.Equal(HttpStatusCode.OK, httpResult.StatusCode);
            _output.WriteLine(cursoAtualizado.ToString());
        }

        [Fact]
        public async Task BuscarCurso_PassarIdCurso_ReceberStatusCode200ECurso()
        {
            //Arrange
            _curso = new Curso()
            {
                IdCurso = _curso.IdCurso
            };

            //Act
            var httpClient = _factory.CreateClient();
            var httpResponse = await httpClient.GetAsync($"api/Cursos/BuscarPorId/{_curso.IdCurso}");

            var cursoEncontrado = JsonConvert.DeserializeObject<Curso>(httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            //Asserts
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.NotNull(cursoEncontrado);
        }

        public async Task InitializeAsync()
        {
            await Criar_InformandoNome_RetornarStatusCode200EObjetoCriadoAsync();
        }

        public async Task DisposeAsync()
        { 
            var httpClient = _factory.CreateClient();
            httpClient.Dispose();
        }
    }
}
