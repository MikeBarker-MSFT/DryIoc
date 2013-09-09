﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DryIoc;
using DryIoc.UnitTests;
using DryIoc.UnitTests.Performance;

namespace ImmutableHashTree_SpeedTest
{
	class Program
	{
		static void Main()
		{
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
            GC.Collect();
            GetDictVsHashTree2OfType(itemCount: 2000);
            Console.WriteLine();
            GetDictVsHashTree2OfType(itemCount: 2000);
            //TypeComparisonSpeedTest(typeof(string));
			Console.ReadKey();
		}

	    private static void TypeComparisonSpeedTest(Type actual)
	    {
            var times = 5 * 1000 * 1000;
	        
            var expected = typeof (string);
	        var result = false;
            Stopwatch time;

	        time = Stopwatch.StartNew();
	        for (int i = 0; i < times; i++)
            {
                result = expected == actual;
            }
            time.Stop();
            Console.WriteLine("type1 == type2 took {0} milliseconds to complete.", time.ElapsedMilliseconds);
            GC.Collect();


            time = Stopwatch.StartNew();
            for (int i = 0; i < times; i++)
            {
                result = expected.Equals(actual);
            }
            time.Stop();
            Console.WriteLine("type1.Equals(type2) took {0} milliseconds to complete.", time.ElapsedMilliseconds);
            GC.Collect();


            time = Stopwatch.StartNew();
            for (int i = 0; i < times; i++)
	        {
                result = Equals(expected, actual);
	        }
            time.Stop();
            Console.WriteLine("Equals(type1, type2) took {0} milliseconds to complete.", time.ElapsedMilliseconds);
            GC.Collect();

            time = Stopwatch.StartNew();
            for (int i = 0; i < times; i++)
            {
                result = ReferenceEquals(expected, actual) || expected.Equals(actual);
            }
            time.Stop();
            Console.WriteLine("ReferenceEquals(type1, type2) || type1.Equals(type2) took {0} milliseconds to complete.", time.ElapsedMilliseconds);
            GC.Collect();


            //var expectedHandle = expected.TypeHandle;
            //var actualHandle = actual.TypeHandle;
            //time = Stopwatch.StartNew();
            //for (int i = 0; i < times; i++)
            //{
            //    result = expectedHandle.Equals(actualHandle);
            //}
            //time.Stop();
            //Console.WriteLine("TypeHandle.Equals(TypeHandle) took {0} milliseconds to complete.", time.ElapsedMilliseconds);
            //GC.Collect();
            GC.KeepAlive(result);
	    }

        public static void GetHashTree2vs1OfInt(int itemCount)
        {
            var key = itemCount;
            var value = "hey";

            var keys = Enumerable.Range(0, itemCount).ToArray();

            var v2 = HashTreeV2<string>.Empty;
            var v1 = HashTree<string>.Empty;

            var v2add = IntTreeV2Add(ref v2, keys, key, value);
            var v1add = IntTreeAdd(ref v1, keys, key, value);

            Console.WriteLine("Adding {0} items (ms):", itemCount);
            Console.WriteLine("v2 - " + v2add);
            Console.WriteLine("v1 - " + v1add);
            Console.WriteLine();

            var getTimes = 1 * 1000 * 1000;

            var v2get = IntTreeV2Get(v2, key, getTimes);
            var v1get = IntTreeGet(v1, key, getTimes);

            Console.WriteLine("Getting one out of {0} items {1:N0} times (ms):", itemCount, getTimes);
            Console.WriteLine("v2 - " + v2get);
            Console.WriteLine("v1 - " + v1get);
        }

	    public static void GetHashTree4vs1ofType(int itemCount)
        {
            var key = typeof(HashTreeTests.DictVsMap);
            var value = "hey";

            var keys = typeof(Dictionary<,>).Assembly.GetTypes().Take(itemCount).ToArray();

            var v4 = HashTree<Type, string>.Empty;
            var v1 = HashTree<Hashed<Type, string>>.Empty;

	        var v4AddTime = HashTree4Add(ref v4, keys, key, value);
	        var v1AddTime = HashTreeAdd(ref v1, keys, key, value);

	        Console.WriteLine("Adding {0} items (ms):", itemCount);
            Console.WriteLine("V4 - " + v4AddTime);
            Console.WriteLine("V1 - " + v1AddTime);
            Console.WriteLine();

            var getTimes = 1 * 1000 * 1000;

	        var v4GetTime = HashTree4Get(v4, key, getTimes);
	        var v1GetTime = HashTreeGet(v1, key, getTimes);

	        Console.WriteLine("Getting one out of {0} items {1:N0} times (ms):", itemCount, getTimes);
            Console.WriteLine("V4 - " + v4GetTime);
            Console.WriteLine("V1 - " + v1GetTime);
        }

        public static void GetHashTree2vs1ofType(int itemCount)
        {
            var key = typeof(HashTreeTests.DictVsMap);
            var value = "hey";

            var keys = typeof(Dictionary<,>).Assembly.GetTypes().Take(itemCount).ToArray();

            var v2 = HashTree2<Type, string>.Empty;
            var v1 = HashTree<Hashed<Type, string>>.Empty;

            var v2AddTime = HashTree2Add(ref v2, keys, key, value);
            var v1AddTime = HashTreeAdd(ref v1, keys, key, value);

            Console.WriteLine("Adding {0} items (ms):", itemCount);
            Console.WriteLine("V2 - " + v2AddTime);
            Console.WriteLine("V1 - " + v1AddTime);
            Console.WriteLine();

            var getTimes = 1 * 1000 * 1000;

            var v2GetTime = HashTree2Get(v2, key, getTimes);
            var v1GetTime = HashTreeGet(v1, key, getTimes);

            Console.WriteLine("Getting one out of {0} items {1:N0} times (ms):", itemCount, getTimes);
            Console.WriteLine("V2 - " + v2GetTime);
            Console.WriteLine("V1 - " + v1GetTime);
        }

        public static void GetDictVsHashTree2OfType(int itemCount)
        {
            var key = typeof(HashTreeTests.DictVsMap);
            var value = "hey";

            var keys = typeof(Dictionary<,>).Assembly.GetTypes().Take(itemCount).ToArray();

            var dict = new Dictionary<Type, string>();
            var tree = HashTree<Type, string>.Empty;

            var dictAddTime = DictAdd(dict, keys, key, value);
            var treeAddTime = HashTree4Add(ref tree, keys, key, value);

            Console.WriteLine("Adding {0} items (ms):", itemCount);
            Console.WriteLine("Dict - " + dictAddTime);
            Console.WriteLine("Tree - " + treeAddTime);
            Console.WriteLine();

            var getTimes = 1 * 1000 * 1000;

            var dictGetTime = DictGet(dict, key, getTimes);
            var treeGetTime = HashTree4Get(tree, key, getTimes);

            Console.WriteLine("Getting one out of {0} items {1:N0} times (ms):", itemCount, getTimes);
            Console.WriteLine("Dict - " + dictGetTime);
            Console.WriteLine("Tree - " + treeGetTime);
        }

		public static void GetDictVsHashTreeOfInt(int itemCount)
		{
			var key = itemCount;
			var value = "hey";

			var keys = Enumerable.Range(0, itemCount).ToArray();

			var dict = new Dictionary<int, string>();
			var tree = HashTree<string>.Empty;

			var dictAddTime = DictAdd(dict, keys, key, value);
			var treeAddTime = IntTreeAdd(ref tree, keys, key, value);

			Console.WriteLine("Adding {0} items (ms):", itemCount);
			Console.WriteLine("Dict - " + dictAddTime);
			Console.WriteLine("Tree - " + treeAddTime);
			Console.WriteLine();

			var getTimes = 1 * 1000 * 1000;

			var dictGetTime = DictGet(dict, key, getTimes);
			var treeGetTime = IntTreeGet(tree, key, getTimes);

			Console.WriteLine("Getting one out of {0} items {1:N0} times (ms):", itemCount, getTimes);
			Console.WriteLine("Dict - " + dictGetTime);
			Console.WriteLine("Tree - " + treeGetTime);
		}

		public static void GetDictVsHashTreeOfType(int itemCount)
		{
			var key = typeof(HashTreeTests.DictVsMap);
			var value = "hey";

			var keys = typeof(Dictionary<,>).Assembly.GetTypes().Take(itemCount).ToArray();

			var dict = new Dictionary<Type, string>();
			var tree = HashTree<Hashed<Type, string>>.Empty;

			var dictAddTime = DictAdd(dict, keys, key, value);
			var treeAddTime = HashTreeAdd(ref tree, keys, key, value);

			Console.WriteLine("Adding {0} items (ms):", itemCount);
			Console.WriteLine("Dict - " + dictAddTime);
			Console.WriteLine("Tree - " + treeAddTime);
			Console.WriteLine();

			var getTimes = 1 * 1000 * 1000;

			var dictGetTime = DictGet(dict, key, getTimes);
			var treeGetTime = HashTreeGet(tree, key, getTimes);

			Console.WriteLine("Getting one out of {0} items {1:N0} times (ms):", itemCount, getTimes);
			Console.WriteLine("Dict - " + dictGetTime);
			Console.WriteLine("Tree - " + treeGetTime);
		}

		private static long DictAdd<K, V>(Dictionary<K, V> dict, K[] keys, K key, V value)
		{
			var addSyncRoot = new object();

			var dictWatch = Stopwatch.StartNew();

			for (var i = 0; i < keys.Length; i++)
				lock (addSyncRoot)
					dict.Add(keys[i], default(V));

			lock (addSyncRoot)
				dict.Add(key, value);

			dictWatch.Stop();
            GC.Collect();
			return dictWatch.ElapsedMilliseconds;
		}

		private static long DictGet<K, V>(Dictionary<K, V> dict, K key, int times)
		{
			V ignored;

			var getSyncRoot = new object();
			var dictWatch = Stopwatch.StartNew();

			for (var i = 0; i < times; i++)
				lock (getSyncRoot)
					dict.TryGetValue(key, out ignored);

			dictWatch.Stop();
            GC.Collect();
            return dictWatch.ElapsedMilliseconds;
		}

	    private static long IntTreeAdd<V>(ref HashTree<V> tree, int[] keys, int key, V value)
		{
			var ignored = default(V);
			var treeTime = Stopwatch.StartNew();

			for (var i = 0; i < keys.Length; i++)
				Interlocked.Exchange(ref tree, tree.AddOrUpdate(keys[i], ignored));

			Interlocked.Exchange(ref tree, tree.AddOrUpdate(key, value));

			treeTime.Stop();
            GC.Collect();
			return treeTime.ElapsedMilliseconds;
		}

        private static long IntTreeV2Add<V>(ref HashTreeV2<V> tree, int[] keys, int key, V value)
        {
            var ignored = default(V);
            var treeTime = Stopwatch.StartNew();

            for (var i = 0; i < keys.Length; i++)
                Interlocked.Exchange(ref tree, tree.AddOrUpdate(keys[i], ignored));

            Interlocked.Exchange(ref tree, tree.AddOrUpdate(key, value));

            treeTime.Stop();
            GC.Collect();
            return treeTime.ElapsedMilliseconds;
        }

		private static long IntTreeGet<V>(HashTree<V> tree, int key, int times)
		{
			V ignored = default(V);
			var treeWatch = Stopwatch.StartNew();

			for (int i = 0; i < times; i++)
                ignored = tree.TryGet(key);

			treeWatch.Stop();
            GC.KeepAlive(ignored);
            GC.Collect();
			return treeWatch.ElapsedMilliseconds;
		}

        private static long IntTreeV2Get<V>(HashTreeV2<V> tree, int key, int times)
        {
            V ignored = default(V);
            var treeWatch = Stopwatch.StartNew();

            for (int i = 0; i < times; i++)
                ignored = tree.TryGet(key);

            treeWatch.Stop();
            GC.KeepAlive(ignored);
            GC.Collect();
            return treeWatch.ElapsedMilliseconds;
        }

        private static long HashTree4Add<V>(ref HashTree<Type, V> tree, Type[] keys, Type key, V value)
        {
            var ignored = default(V);
            var treeTime = Stopwatch.StartNew();

            for (var i = 0; i < keys.Length; i++)
            {
                var k = keys[i];
                Interlocked.Exchange(ref tree, tree.AddOrUpdate(k, ignored));
            }

            Interlocked.Exchange(ref tree, tree.AddOrUpdate(key, value));

            treeTime.Stop();
            GC.KeepAlive(ignored);
            GC.Collect();
            return treeTime.ElapsedMilliseconds;
        }

        private static long HashTree2Add<V>(ref HashTree2<Type, V> tree, Type[] keys, Type key, V value)
        {
            var ignored = default(V);
            var treeTime = Stopwatch.StartNew();

            for (var i = 0; i < keys.Length; i++)
            {
                var k = keys[i];
                Interlocked.Exchange(ref tree, tree.AddOrUpdate(k, ignored));
            }

            Interlocked.Exchange(ref tree, tree.AddOrUpdate(key, value));

            treeTime.Stop();
            GC.KeepAlive(ignored);
            GC.Collect();
            return treeTime.ElapsedMilliseconds;
        }

		private static long HashTreeAdd<V>(ref HashTree<Hashed<Type, V>> tree, Type[] keys, Type key, V value)
		{
			var ignored = default(V);
			var treeTime = Stopwatch.StartNew();

			for (var i = 0; i < keys.Length; i++)
			{
				var k = keys[i];
				Interlocked.Exchange(ref tree, tree.AddOrUpdate(k, ignored));
			}

			Interlocked.Exchange(ref tree, tree.AddOrUpdate(key, value));

			treeTime.Stop();
            GC.KeepAlive(ignored);
            GC.Collect();
			return treeTime.ElapsedMilliseconds;
		}

        private static long HashTree4Get<T>(HashTree<Type, T> tree, Type key, int times)
        {
            T ignored = default(T);

            var treeWatch = Stopwatch.StartNew();

            for (int i = 0; i < times; i++)
            {
                ignored = tree.TryGet(key);
            }

            treeWatch.Stop();
            GC.KeepAlive(ignored);
            GC.Collect();
            return treeWatch.ElapsedMilliseconds;
        }

        private static long HashTree2Get<T>(HashTree2<Type, T> tree, Type key, int times)
        {
            T ignored = default(T);

            var treeWatch = Stopwatch.StartNew();

            for (int i = 0; i < times; i++)
            {
                ignored = tree.TryGet(key);
            }

            treeWatch.Stop();
            GC.KeepAlive(ignored);
            GC.Collect();
            return treeWatch.ElapsedMilliseconds;
        }

        private static long HashTreeGet<T>(HashTree<Hashed<Type, T>> tree, Type key, int times)
		{
			T ignored = default(T);

			var treeWatch = Stopwatch.StartNew();

			for (int i = 0; i < times; i++)
			{
				ignored = tree.TryGet(key);
			}

			treeWatch.Stop();
            GC.KeepAlive(ignored);
            GC.Collect();
			return treeWatch.ElapsedMilliseconds;
		}
	}
}
