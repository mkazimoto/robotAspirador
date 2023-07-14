using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;

namespace ConsoleApp2
{
  class Program
  {
    /// <summary>
    /// Mapa da sala com obstáculos
    /// </summary>
    static char[,] mapaSala = new char[,]
    {
            { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', '#', '#', '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', '#', '#', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', '#', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', '#', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', '#', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', '#', '#', ' ', ' ', ' ', '#' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', '#', '#' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ' },
            { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ' },
            { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', ' ', ' ' }
    };

    /// <summary>
    /// Log de execução
    /// </summary>
    static StringBuilder sbOutput;

    /// <summary>
    /// Pilha de pontos para memorizar as posições pendentes
    /// </summary>
    static Stack<Point> pilhaPontosPendentes;

    static void Main(string[] args)
    {
      Console.ReadKey();

      sbOutput = new StringBuilder();

      pilhaPontosPendentes = new Stack<Point>();

      var posicaoInicial = new Point(1, 1);

      sbOutput.AppendLine("Executando limpeza da sala...");

      var posicaoFinal = ExecutarLimpeza(posicaoInicial);

      sbOutput.AppendLine("Limpeza finalizada!");

      ImprimirSala(mapaSala);

      Thread.Sleep(3000);

      sbOutput.AppendLine("Voltando para a base de carregamento...");

      MoverAtePonto(posicaoFinal, posicaoInicial);

      sbOutput.AppendLine("Carregando bateria...");

      ImprimirSala(mapaSala);

      Console.ReadKey();
    }

    /// <summary>
    /// Executa o movimento de limpeza do robõ
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    static Point ExecutarLimpeza(Point posicaoInicial)
    {
      var posicaoAtual = posicaoInicial;
      Point posicaoAnterior;

      
      EmpilhaPosicaoPendente(posicaoAtual, mapaSala);

      while (pilhaPontosPendentes.Count > 0)
      {
        var posicaoProxima = pilhaPontosPendentes.Pop();

        // Verificar se a posição atual é válida
        if (mapaSala[posicaoProxima.Y, posicaoProxima.X] != ' ')
        {
          continue;
        }

        posicaoAnterior = posicaoAtual;
        posicaoAtual = posicaoProxima;

        // A próxima posição não é vizinha ?
        if (Math.Abs(posicaoAnterior.X - posicaoAtual.X) > 1 ||
            Math.Abs(posicaoAnterior.Y - posicaoAtual.Y) > 1)
        {
          // Deve pegar o menor caminho (Dijkstra) para evitar o salto de posição
          MoverAtePonto(posicaoAnterior, posicaoAtual);
        }

        // Imprimir robô aspirador
        mapaSala[posicaoAtual.Y, posicaoAtual.X] = 'O';

        ImprimirSala(mapaSala);

        // Marcar a posição atual como visitada
        mapaSala[posicaoAtual.Y, posicaoAtual.X] = '*';

        EmpilhaPosicaoPendente(new Point(posicaoAtual.X, posicaoAtual.Y + 1), mapaSala); // Baixo
        EmpilhaPosicaoPendente(new Point(posicaoAtual.X, posicaoAtual.Y - 1), mapaSala); // Cima
        EmpilhaPosicaoPendente(new Point(posicaoAtual.X + 1, posicaoAtual.Y), mapaSala); // Direita
        EmpilhaPosicaoPendente(new Point(posicaoAtual.X - 1, posicaoAtual.Y), mapaSala); // Esquerda
      }

      mapaSala[posicaoAtual.Y, posicaoAtual.X] = 'O';
      ImprimirSala(mapaSala);

      return posicaoAtual;
    }

    static void MoverAtePonto(Point posicaoInicial, Point posicaoFinal)
    {
      var dijkstra = new Dijkstra(mapaSala);
      List<Point> path = dijkstra.GetPath(posicaoInicial, posicaoFinal);

      // Movimenta até a posição pendente
      foreach (var position in path)
      {
        // Imprimir robô aspirador
        mapaSala[position.Y, position.X] = 'O';

        ImprimirSala(mapaSala);

        if (position == posicaoFinal)
        {
          break;
        }

        // Marcar a posição atual como visitada
        mapaSala[position.Y, position.X] = '*';
      }
    }

    static void EmpilhaPosicaoPendente(Point posicao, char[,] mapaSala)
    {
      // Verificar se a posição atual é válida
      if (mapaSala[posicao.Y, posicao.X] != ' ')
      {
        return;
      }
      pilhaPontosPendentes.Push(posicao);
    }

    static void ImprimirSala(char[,] mapaSala)
    {
      Console.Clear();

      int altura = mapaSala.GetLength(0);
      int largura = mapaSala.GetLength(1);

      for (int y = 0; y < altura; y++)
      {
        for (int x = 0; x < largura; x++)
        {
          Console.Write(mapaSala[y, x]);
        }
        Console.WriteLine();
      }

      Console.WriteLine(sbOutput.ToString());

      Thread.Sleep(100);
    }
  }
}
