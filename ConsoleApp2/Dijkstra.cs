using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
  /// <summary>
  /// Algoritimo do menor caminho Dijkstra A*
  /// </summary>
  public class Dijkstra
  {
    private int maxY;
    private int maxX;
    private char[,] mapaSala;
    
    public Dijkstra(char[,] mapaSala)
    {
      maxY = mapaSala.GetLength(0);
      maxX = mapaSala.GetLength(1);
      this.mapaSala = mapaSala;
    }

    /// <summary>
    /// Retorna o menor caminho entre 2 pontos
    /// </summary>
    /// <param name="posicaoInicial"></param>
    /// <param name="posicaoFinal"></param>
    /// <returns></returns>
    public List<Point> GetPath(Point posicaoInicial, Point posicaoFinal)
    {
      // Pilha de pontos para memorizar as posições pendentes
      Stack<Point> pilhaPosicao = new Stack<Point>();

      int totalNodos = maxY * maxX;

      int[] distance = new int[totalNodos];
      int[] parent = new int[totalNodos];
      bool[] visited = new bool[totalNodos];

      for (int i = 0; i < totalNodos; i++)
      {
        distance[i] = int.MaxValue;
        parent[i] = -1;
        visited[i] = false;
      }

      int sourceIndex = GetIndex(posicaoInicial);
      int destinationIndex = GetIndex(posicaoFinal);

      distance[sourceIndex] = 0;

      pilhaPosicao.Push(posicaoInicial);

      while (!visited[destinationIndex] && pilhaPosicao.Count > 0)
      {
        var position = pilhaPosicao.Pop();
        int positionIndex = GetIndex(position);

        // Marca nodo como visitado
        visited[positionIndex] = true;

        List<Point> listProximaPosicao = GetProximaPosicao(position, posicaoFinal, visited);

        foreach (var proximo in listProximaPosicao)
        {
          int neighborIndex = GetIndex(proximo);
          int totalDistance = distance[positionIndex] + 1;

          if (!visited[neighborIndex] && 
              mapaSala[proximo.Y, proximo.X] != '#' && 
              totalDistance < distance[neighborIndex])
          {
            pilhaPosicao.Push(proximo);

            distance[neighborIndex] = totalDistance;
            parent[neighborIndex] = positionIndex;
          }
        }
      }

      return ReconstructPath(parent, posicaoInicial, posicaoFinal);
    }

    /// <summary>
    /// Retorna o índice do ponto
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private int GetIndex(Point position)
    {
      return position.Y * maxX + position.X;
    }

    /// <summary>
    /// Retorna o ponto do índice
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Point GetCoordinates(int index)
    {
      int row = index / maxX;
      int column = index % maxX;
      return new Point(column, row);
    }
    
    /// <summary>
    /// Retorna as posições vizinhas
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    private List<Point> GetProximaPosicao(Point position, Point destination, bool[] visited)
    {
      List<(Point position, double distance)> neighbors = new List<(Point position, double distance)>();

      // Considerando movimentos nos sentidos: cima, baixo, esquerda e direita
      int[] dX = { -1, 1, 0, 0 };
      int[] dY = { 0, 0, -1, 1 };

      for (int i = 0; i < 4; i++)
      {
        var newPosition = new Point(position.X + dX[i], position.Y + dY[i]);

        if (IsValidPosition(newPosition, visited))
        {
          var distance = GetDistance(newPosition, destination);
          neighbors.Add((position: newPosition, distance: distance));
        }
      }

      // Prioriza a menor distancia até o ponto de destino
      return neighbors.OrderByDescending(p => p.distance).Select(p => p.position).ToList();
    }

    /// <summary>
    /// Calcula a distancia entre 2 pontos (Teorema de Pitágoras)
    /// </summary>
    /// <param name="position1"></param>
    /// <param name="position2"></param>
    /// <returns></returns>
    private double GetDistance(Point position1, Point position2)
    {
      return Math.Sqrt(Math.Pow(Math.Abs(position2.X - position1.X), 2) + Math.Pow(Math.Abs(position2.Y - position1.Y), 2));
    }

    /// <summary>
    /// Verifica se é uma posição válida no mapa
    /// </summary>
    /// <param name="position"></param>
    /// <param name="visited"></param>
    /// <returns></returns>
    private bool IsValidPosition(Point position, bool[] visited)
    {
      int index = GetIndex(position);
      return position.Y >= 0 && 
             position.Y < maxY && 
             position.X >= 0 && 
             position.X < maxX && 
             mapaSala[position.Y, position.X] != '#' && 
             !visited[index];
    }

    /// <summary>
    /// Extrai o caminho de pontos
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public List<Point> ReconstructPath(int[] parent, Point source, Point destination)
    {
      List<Point> path = new List<Point>();
      int current = GetIndex(destination);

      while (current != -1 && current != GetIndex(source) )
      {
        path.Add(GetCoordinates(current));
        current = parent[current];
      }

      path.Add(source);
      path.Reverse();

      return path;
    }
  }
  
}
