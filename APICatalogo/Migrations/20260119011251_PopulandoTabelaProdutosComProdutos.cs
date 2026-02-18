using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    /// <inheritdoc />
    public partial class PopulandoTabelaProdutosComProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("insert into Produtos (Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) " +
                   "values ('Coca-Cola Lata', 'Refrigerante sabor cola 350ml', 5.00, 'coca_cola_lata.jpg', 100, now(),1)");
            mb.Sql("insert into Produtos (Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) " +
                     "values ('Hambúrguer Simples', 'Hambúrguer com queijo, alface, tomate e molho especial', 15.00, 'hamburguer_simples.jpg', 50, now(),1)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Produtos");

        }
    }
}
