using System;
using System.Collections.Generic;

namespace znajdowanieTrasyAStar
{
    public class Program
    {
		public static List<List<Node>> grid = new List<List<Node>>();
		public static List<Node> UkonczoneSciezka;
		public const int WielkoscX = 10;
		public const int WielkoscY = 10;

        static void Main(string[] args)
        {
			Random rnd = new Random();
            for (int x = 0; x < WielkoscX; x++)
            {
				List<Node> gridX = new List<Node>();
                for (int y = 0; y < WielkoscY; y++)
                {
					gridX.Add(new Node(new Vector2(x, y), rnd.Next(10)>=3));
                }
				grid.Add(gridX);
            }

			Pokaz();
			FindPath(grid[0][0], grid[WielkoscX - 1][WielkoscY - 1]);
            Console.WriteLine("\n\n");
			Pokaz();
		}

		static public void Pokaz()
        {
			if (UkonczoneSciezka != null)
			{
				for (int i = 0; i < UkonczoneSciezka.Count; i++)
				{
					UkonczoneSciezka[i].znak = 'O';
				}
			}
			for (int x = 0; x < WielkoscX; x++)
            {
                for (int y = 0; y < WielkoscY; y++)
                {
					Console.Write($" { (grid[x][y].moznaChodzic? grid[x][y].znak : '#')}");
                }
				Console.WriteLine();
            }
        }

		static void FindPath(Node startNode, Node koniecNode)
		{
			List<Node> openSet = new List<Node>();
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);

			while (openSet.Count > 0)
			{
				Node node = openSet[0];
				for (int i = 1; i < openSet.Count; i++)
				{
					if (openSet[i].fKoszt < node.fKoszt || openSet[i].fKoszt == node.fKoszt)
					{
						if (openSet[i].hKoszt < node.hKoszt)
							node = openSet[i];
					}
				}

				openSet.Remove(node);
				closedSet.Add(node);

				if (node == koniecNode)
				{
					RetracePath(startNode, koniecNode);
					return;
				}

				foreach (Node neighbour in Node.ZdobadziSasiadow(node))
				{
					if (!neighbour.moznaChodzic || closedSet.Contains(neighbour))
					{
						continue;
					}

					int newCostToNeighbour = node.gKoszt + GetDistance(node, neighbour);
					if (newCostToNeighbour < neighbour.gKoszt || !openSet.Contains(neighbour))
					{
						neighbour.gKoszt = newCostToNeighbour;
						neighbour.hKoszt = GetDistance(neighbour, koniecNode);
						neighbour.rodzic = node;

						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}

		static void RetracePath(Node startNode, Node endNode)
		{
			List<Node> path = new List<Node>();
			Node currentNode = endNode;

			while (currentNode != startNode)
			{
				path.Add(currentNode);
				currentNode = currentNode.rodzic;
			}
			path.Reverse();

			UkonczoneSciezka = path;
		}

		static int GetDistance(Node nodeA, Node nodeB)
		{
			int dstX = Math.Abs(nodeA.polozenie.x - nodeB.polozenie.x);
			int dstY = Math.Abs(nodeA.polozenie.y - nodeB.polozenie.y);

			if (dstX > dstY)
				return 14 * dstY + 10 * (dstX - dstY);
			return 14 * dstX + 10 * (dstY - dstX);
		}
	}

    public class Vector2
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector2(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    public class Node
    {
        public Vector2 polozenie;
        public bool moznaChodzic;

        public int gKoszt;
        public int hKoszt;
        public Node rodzic;

		public char znak = ' ';

        public Node(Vector2 _polozenie, bool _moznaChodzic)
        {
            polozenie = _polozenie;
            moznaChodzic = _moznaChodzic;
        }

        public int fKoszt { get { return gKoszt + hKoszt; } }

		public static List<Node> ZdobadziSasiadow(Node node)
        {
			List<Node> nody = new List<Node>();

			if (node.polozenie.x < Program.WielkoscX - 1)
			{
				nody.Add(Program.grid[node.polozenie.x + 1][node.polozenie.y]);
			}
			if (node.polozenie.x > 0)
			{
				nody.Add(Program.grid[node.polozenie.x - 1][node.polozenie.y]);
			}
			if (node.polozenie.y < Program.WielkoscY - 1)
			{
				nody.Add(Program.grid[node.polozenie.x][node.polozenie.y + 1]);
			}
			if (node.polozenie.y > 0)
			{
				nody.Add(Program.grid[node.polozenie.x][node.polozenie.y - 1]);
			}

			return nody;
        }
    }
}
