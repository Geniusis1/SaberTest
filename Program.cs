using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SaberTest
{
	class Program
	{
		private static int _COUNT = 5;
		private static string _fileName = "file";

		static void Main(string[] args)
		{
			ListRandom lr = new ListRandom();
			ListNode[] listNode = new ListNode[_COUNT];

			//Init list
			for (int i = 0; i < _COUNT; i++) listNode[i] = new ListNode();
			for (int i = 0; i < _COUNT; i++)
			{
				listNode[i].Data = "kqw" + i;
				listNode[i].Random = listNode[new Random().Next(_COUNT)];

				if (i != 0 && i != _COUNT - 1)
				{
					listNode[i].Next = listNode[i + 1];
					listNode[_COUNT - i - 1].Previous = listNode[_COUNT - i - 2];
				}
			}
			
			lr.Count = _COUNT;

			lr.Head = listNode[0];
			lr.Head.Next = listNode[1];

			lr.Tail = listNode[_COUNT - 1];
			lr.Tail.Previous = listNode[_COUNT - 2];

			//Delete file
			if (File.Exists(_fileName))
			{
				File.Delete(_fileName);
			}

			//Serialize
			FileStream stFile = File.Open(_fileName, FileMode.Create);
			lr.Serialize(stFile);
			stFile.Close();

			Console.WriteLine("-----------------list-1---------------");
			CheckList(lr);

			//Deserialize
			lr = new ListRandom();
			stFile = File.Open(_fileName, FileMode.Open);
			lr.Deserialize(stFile);
			stFile.Close();

			Console.WriteLine("-----------------list-2---------------");
			CheckList(lr);
		}

		private static void CheckList(ListRandom lr)
		{
			ListNode cursor = lr.Head;

			Console.WriteLine("id\tdata\tpre\tnext\trand");
			for (int i = 0; i < _COUNT; i++)
			{
				Console.Write(i + "\t");
				Console.Write(cursor.Data + '\t');
				try
				{
					Console.Write(cursor.Previous.Data + '\t');
				}
				catch(Exception)
				{
					Console.Write("NULL" + '\t');
				}
				try
				{
					Console.Write(cursor.Next.Data + '\t');
				} 
				catch(Exception)
				{
					Console.Write("NULL" + '\t');
				}
				Console.Write(cursor.Random.Data + '\n');

				cursor = cursor.Next;
			}
		}
	}

	class ListNode
	{
		public ListNode Previous;
		public ListNode Next;
		public ListNode Random; // произвольный элемент внутри списка
		public string Data;
	}

	class ListRandom
	{
		public ListNode Head;
		public ListNode Tail;
		public int Count;

		public void Serialize(Stream s)
		{
			byte[] bytes;
			byte[] separ = Encoding.ASCII.GetBytes("?");

			s.Write(Encoding.ASCII.GetBytes(Count + ""));

			ListNode cursor = Head;

			for (int i = 0; i < Count; i++)
			{
				bytes = Encoding.ASCII.GetBytes(cursor.Data);

				s.Write(separ);
				s.Write(Encoding.ASCII.GetBytes(bytes.Length + ""));
				s.Write(separ);
				s.Write(bytes);

				ListNode sec = Head;

				for (int j = 0; j < Count; j++)
				{
					if (sec == cursor.Random)
					{
						s.Write(Encoding.ASCII.GetBytes(j + ""));
						break;
					}
					sec = sec.Next;
				}
				cursor = cursor.Next;
			}
			s.Write(separ);
			s.Flush();
		}

		public void Deserialize(Stream s)
		{
			string str;
			byte[] buf = new byte[s.Length];

			s.Read(buf, 0, (int)s.Length);
			str = Encoding.ASCII.GetString(buf);

			Count = Int32.Parse(str.Split("?", 2)[0]);
			str = str.Remove(0, str.Split("?", 2)[0].Length + 1);
			ListNode[] listNodes = new ListNode[Count];

			for (int i = 0; i < Count; i++) listNodes[i] = new ListNode();
			for (int i = 0; i < Count; i++)
			{
				int count;

				count = Int32.Parse(str.Split("?", 2)[0]);
				str = str.Remove(0, str.Split("?", 2)[0].Length + 1);

				listNodes[i].Data = str.Substring(0, count);
				str = str.Remove(0, count);

				listNodes[i].Random = listNodes[Int32.Parse(str.Split("?", 2)[0])];
				str = str.Remove(0, str.Split("?", 2)[0].Length + 1);

				if (i != 0 && i != Count - 1)
				{
					listNodes[i].Next = listNodes[i + 1];
					listNodes[Count - i - 1].Previous = listNodes[Count - i - 2];
				}
			}

			Head = listNodes[0];
			Head.Next = listNodes[1];

			Tail = listNodes[Count - 1];
			Tail.Previous = listNodes[Count - 2];
		}
	}
}