using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace LINQ_Extensions.Test
{
	[TestFixture]
    public class LazyMaterializerTest
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

			var lazyMaterializer = new LazyMaterializer<int>(src);

			var first = lazyMaterializer.First();
			var firstAgain = lazyMaterializer.First();

			Assert.AreEqual(1, _createdItems.Count);
			Assert.AreEqual(first, firstAgain);
		}

		[Test]
		public void TestItemsNotCreatedTwiceMultiple()
		{
			var src = GetCollection();

			var lazyMaterializer = new LazyMaterializer<int>(src);

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

			var lazyMaterializer = new LazyMaterializer<int>(src);

			lazyMaterializer.First();
			
			Assert.AreEqual(1, _createdItems.Count);
		}


		[Test]
		public void TestEnumerableLazilyEvaluated_2()
		{
			var src = GetCollection();

			var lazyMaterializer = new LazyMaterializer<int>(src);

			lazyMaterializer.Take(5);

			Assert.AreEqual(0, _createdItems.Count);
		}

	    [Test]
	    public void EnumeratingCollectionTwice()
	    {
            var src = GetCollection();

            var lazyMaterializer = new LazyMaterializer<int>(src);
	        var firstSet = new List<int>();
	        foreach (var item in lazyMaterializer)
	        {
	            firstSet.Add(item);
	            if (item == 5)
	                break;
	        }
            var secondSet = new List<int>();
            foreach (var item in lazyMaterializer)
            {
                secondSet.Add(item);
                if (item == 5)
                    break;
            }
            
            Assert.True(firstSet.SequenceEqual(secondSet));
            Assert.AreEqual(6, _createdItems.Count);
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
