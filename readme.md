## V1 da Integracao ASP.NET CORE com MYSQL

Apenas uma simples conexão de um DB com o ASP.NET CORE. Ele conecta, e os resultados vão para a Index, imprimindo na tela.
A ideia é que o banco de dados seja manipulável via web. A base é essa.

## O que foi utilizado?

<ul>
<li>.NET Framework 8.0li>
<li><TargetFramework>net8.0</TargetFramework></li>
<br>
<li>MySQL 9.1.0</li>
<li><PackageReference Include="MySql.Data" Version="9.1.0" /></li>
</ul>

Também desativei a <b>Nullable</b> pois estava dando alguns problemas, mas vou contornar e reativar nos próximos commits. 
O banco de dados é fictício, hospedado por mim. Não há nenhuma informação sensível nele, e qualquer um pode conectar-se para fins educacionais.
