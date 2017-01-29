using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace LINQ_Extensions.Test
{
	[TestFixture]
    public class CachedEnumerableTest
	{
		private readonly List<int> _createdItems = new List<int>();

		[SetUp]
	    public void Setup()
	    {
			_createdItems.Clear();
	    }

		[Test]
	    public void TestItemsNotCreatedTwiceSingle()
		{
			var src = GetCollection();

			var lazyMaterializer = new CachedEnumerable<int>(src);

			var first = lazyMaterializer.First();
			var firstAgain = lazyMaterializer.First();

			Assert.AreEqual(1, _createdItems.Count);
			Assert.AreEqual(first, firstAgain);
		}

		[Test]
		public void TestItemsNotCreatedTwiceMultiple()
		{
			var src = GetCollection();

			var lazyMaterializer = new CachedEnumerable<int>(src);

			var first = lazyMaterializer.Take(5).ToList();
			var second = lazyMaterializer.Take(3).ToList();

			Assert.AreEqual(5, _createdItems.Count);
			Assert.True(src.Take(5).SequenceEqual(first));
			Assert.True(first.Take(3).SequenceEqual(second));
		}

		[Test]
	    public void TestEnumerableLazilyEvaluated()
	    {
			var src = GetCollection();

			var lazyMaterializer = new CachedEnumerable<int>(src);

			lazyMaterializer.First();
			
			Assert.AreEqual(1, _createdItems.Count);
		}


		[Test]
		public void TestEnumerableLazilyEvaluated_2()
		{
			var src = GetCollection();

			var lazyMaterializer = new CachedEnumerable<int>(src);

			lazyMaterializer.Take(5);

			Assert.AreEqual(0, _createdItems.Count);
		}

	    [Test]
	    public void EnumeratingCollectionTwice()
	    {
            var src = GetCollection();

            var lazyMaterializer = new CachedEnumerable<int>(src);
	        var first = lazyMaterializer.Take(1).ToList();
            var second = lazyMaterializer.Take(2).ToList();

	        Assert.True(first.SequenceEqual(new[] {0}));
	        Assert.True(second.SequenceEqual(new[] {0, 1}));
            Assert.AreEqual(2, _createdItems.Count);
        }

	    [Test]
	    public void EnumeratingNested()
	    {
	        var src = GetCollection().Take(2);

            var lazyMaterializer = new CachedEnumerable<int>(src);

	        var items = new List<int>();
	        foreach (var outer in lazyMaterializer)
	        {
	            foreach (var inner in lazyMaterializer)
	            {
	                items.Add(outer + inner);
	            }
	        }

            Assert.AreEqual(4, items.Count);
        }

        private IEnumerable<int> GetCollection()
	    {
		    var i = 0;
		    while (true)
		    {
				_createdItems.Add(i);
			    yield return i++;
		    }
	    }
    }
}
