using InsurancePolicyManager.Domain.Entities;

namespace InsurancePolicyManager.Infrastructure.Persistence;

public static class SeedData
{
  private static readonly string[] PrimeirosNomes =
  {
    "Ana", "Bruno", "Carla", "Daniel", "Eduarda", "Felipe", "Gabriela", "Henrique",
    "Isabela", "João", "Karina", "Lucas", "Mariana", "Nicolas", "Olívia", "Pedro",
    "Raquel", "Rodrigo", "Sofia", "Thiago", "Vanessa", "William", "Yasmin", "Zeca",
    "Beatriz", "Caio", "Débora", "Enzo", "Fernanda", "Gustavo", "Helena", "Igor",
    "Julia", "Kevin", "Larissa", "Marcelo"
  };

  private static readonly string[] Sobrenomes =
  {
    "Silva", "Souza", "Oliveira", "Santos", "Pereira", "Costa", "Rodrigues", "Almeida",
    "Nascimento", "Lima", "Araújo", "Fernandes", "Carvalho", "Gomes", "Martins", "Rocha",
    "Ribeiro", "Alves", "Monteiro", "Barbosa", "Cardoso", "Teixeira", "Moreira", "Correia"
  };

  public static void Seed(AppDbContext context)
  {
    if (context.Clientes.Any())
      return;

    var random = new Random(42);
    var ano = DateTime.UtcNow.Year;

    var clientes = new List<Cliente>();
    for (var i = 0; i < 70; i++)
    {
      var nome = $"{PrimeirosNomes[random.Next(PrimeirosNomes.Length)]} {Sobrenomes[random.Next(Sobrenomes.Length)]}";
      var documento = GerarCpfUnico(random, clientes);
      clientes.Add(new Cliente(documento, nome));
    }

    context.Clientes.AddRange(clientes);
    context.SaveChanges();

    var apolices = new List<Apolice>();
    var numeroSequencial = 1;

    for (var i = 0; i < 100; i++)
    {
      var cliente = clientes[random.Next(clientes.Count)];
      var numero = $"SEG-{ano}-{numeroSequencial:D4}";
      numeroSequencial++;

      var placa = GerarPlaca(random);
      var valorPremio = Math.Round((decimal)(random.Next(8000, 45000) / 100.0), 2);

      var perfil = random.Next(0, 100);
      DateTime dataInicio;
      DateTime dataFim;

      if (perfil < 15)
      {
        dataInicio = DateTime.UtcNow.AddMonths(-random.Next(6, 14));
        dataFim = DateTime.UtcNow.AddDays(-random.Next(1, 90));
      }
      else if (perfil < 30)
      {
        dataInicio = DateTime.UtcNow.AddMonths(-random.Next(1, 11));
        dataFim = DateTime.UtcNow.AddDays(random.Next(1, 30));
      }
      else
      {
        dataInicio = DateTime.UtcNow.AddMonths(-random.Next(0, 10));
        dataFim = dataInicio.AddMonths(random.Next(6, 18));
      }

      var apolice = new Apolice(numero, cliente.Id, placa, valorPremio, dataInicio, dataFim);

      if (perfil < 15)
        apolice.Expirar();
      else if (perfil >= 85)
        apolice.Cancelar();

      apolices.Add(apolice);
    }

    context.Apolices.AddRange(apolices);
    context.SaveChanges();
  }

  private static string GerarCpfUnico(Random random, List<Cliente> clientesExistentes)
  {
    string documento;
    do
    {
      documento = GerarCpf(random);
    } while (clientesExistentes.Any(c => c.Documento == documento));

    return documento;
  }

  private static string GerarCpf(Random random)
  {
    var digitos = new int[9];
    for (var i = 0; i < 9; i++)
      digitos[i] = random.Next(0, 10);

    var mult1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    var mult2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

    var soma = 0;
    for (var i = 0; i < 9; i++)
      soma += digitos[i] * mult1[i];

    var resto = soma % 11;
    var dv1 = resto < 2 ? 0 : 11 - resto;

    var comDv1 = digitos.Append(dv1).ToArray();
    soma = 0;
    for (var i = 0; i < 10; i++)
      soma += comDv1[i] * mult2[i];

    resto = soma % 11;
    var dv2 = resto < 2 ? 0 : 11 - resto;

    return string.Concat(digitos) + dv1 + dv2;
  }

  private static string GerarPlaca(Random random)
  {
    var letras = new char[3];
    for (var i = 0; i < 3; i++)
      letras[i] = (char)('A' + random.Next(0, 26));

    var letraMercosul = (char)('A' + random.Next(0, 26));

    return $"{new string(letras)}{random.Next(0, 10)}{letraMercosul}{random.Next(0, 10)}{random.Next(0, 10)}";
  }
}
