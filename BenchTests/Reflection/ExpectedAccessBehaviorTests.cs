
/*
using System.Diagnostics;
using System.Drawing;
using Jay.Reflection;
using static InlineIL.IL;
using static InlineIL.IL.Emit;

namespace Tests
{
    public class ExpectedAccessBehaviorTests
    {
        private static void MutateSet(TestStruct testStruct)
        {
            testStruct = default;
        }
        private static void MutateSetIn(in TestStruct testStruct)
        {
            Ldarg(nameof(testStruct));
            Initobj(typeof(TestStruct));            
        }
        private static void MutateSetRef(ref TestStruct testStruct)
        {
            testStruct = default;
        }
        private static void MutateSetOut(out TestStruct testStruct)
        {
            testStruct = default;
        }
        
        private static void MutateProp(TestStruct testStruct)
        {
            testStruct.Id = 147;
            testStruct.Name = "Mutated";
            testStruct.Guid = Guid.NewGuid();
        }
        private static void MutatePropIn(in TestStruct testStruct)
        {
            throw new InvalidOperationException();     
        }
        private static void MutatePropInForce(in TestStruct testStruct)
        {
            ref TestStruct temp = ref Abuse.Hack.AsRef(in testStruct);
            temp.Id = 147;
            temp.Name = "Mutated";
            temp.Guid = Guid.NewGuid();           
        }
        private static void MutatePropRef(ref TestStruct testStruct)
        {
            testStruct.Id = 147;
            testStruct.Name = "Mutated";
            testStruct.Guid = Guid.NewGuid();
        }
        private static void MutatePropOutForce(out TestStruct testStruct)
        {
            // testStruct = new TestStruct(147)
            // {
            //     Name = "Mutated",
            //     Guid = Guid.NewGuid(),
            // };
            ref TestStruct temp = ref Abuse.Hack.OutAsRef(out testStruct);
            temp.Id = 147;
            temp.Name = "Mutated";
            //temp.Guid = Guid.NewGuid();  
        }
        
        #region MutateSet
        [Fact]
        public void TestMutateSetStruct()
        {
            var guid = Guid.NewGuid();
            var test = new TestStruct(13)
            {
                Name = "Original",
                Guid = guid,
            };
            Assert(() => test.Id == 13 && 
                         test.Name == "Original" &&
                         test.Guid == guid);
            // This should not do anything
            MutateSet(test);
            Assert(() => test.Id == 13 && 
                         test.Name == "Original" &&
                         test.Guid == guid);
        }
        
        [Fact]
        public void TestMutateSetNonInStruct()
        {
            var guid = Guid.NewGuid();
            var test = new TestStruct(13)
            {
                Name = "Original",
                Guid = guid,
            };
            Assert(() => test.Id == 13 && 
                         test.Name == "Original" &&
                         test.Guid == guid);
            // This is abusing the ability to set in params
            MutateSetIn(test);
            Assert(() => test.Id == default && 
                         test.Name == default &&
                         test.Guid == default);
        }
        
        [Fact]
        public void TestMutateSetInStruct()
        {
            var guid = Guid.NewGuid();
            var test = new TestStruct(13)
            {
                Name = "Original",
                Guid = guid,
            };
            Assert(() => test.Id == 13 && 
                         test.Name == "Original" &&
                         test.Guid == guid);
            // This is abusing the ability to set in params
            MutateSetIn(in test);
            Assert(() => test.Id == default && 
                         test.Name == default &&
                         test.Guid == default);
        }
        
        [Fact]
        public void TestMutateSetRefStruct()
        {
            var guid = Guid.NewGuid();
            var test = new TestStruct(13)
            {
                Name = "Original",
                Guid = guid,
            };
            Assert(() => test.Id == 13 && 
                         test.Name == "Original" &&
                         test.Guid == guid);
            // This is using ref as should be
            MutateSetRef(ref test);
            Assert(() => test.Id == default && 
                         test.Name == default &&
                         test.Guid == default);
        }
        #endregion
        
        #region MutateSet
        [Fact]
        public void TestMutatePropStruct()
        {
            var guid = Guid.NewGuid();
            var test = new TestStruct(13)
            {
                Name = "Original",
                Guid = guid,
            };
            Assert(() => test.Id == 13 && 
                         test.Name == "Original" &&
                         test.Guid == guid);
            // This should not do anything -- structs are copied
            MutateProp(test);
            Assert(() => test.Id == 13 && 
                         test.Name == "Original" &&
                         test.Guid == guid);
        }
        
        // [Fact]
        // public void TestMutatePropNonInStruct()
        // {
        //     var guid = Guid.NewGuid();
        //     var test = new TestStruct(13)
        //     {
        //         Name = "Original",
        //         Guid = guid,
        //     };
        //     Assert(() => test.Id == 13 && 
        //                  test.Name == "Original" &&
        //                  test.Guid == guid);
        //     // This is abusing the ability to set in params
        //     MutatePropIn(test);
        //     Assert(() => test.Id == 147 && 
        //                  test.Name == "Mutated" &&
        //                  test.Guid != guid);
        // }
        
        // [Fact]
        // public void TestMutatePropInStruct()
        // {
        //     var guid = Guid.NewGuid();
        //     var test = new TestStruct(13)
        //     {
        //         Name = "Original",
        //         Guid = guid,
        //     };
        //     Assert(() => test.Id == 13 && 
        //                  test.Name == "Original" &&
        //                  test.Guid == guid);
        //     // This is abusing the ability to set in params
        //     MutatePropIn(test);
        //     Assert(() => test.Id == 147 && 
        //                  test.Name == "Mutated" &&
        //                  test.Guid != guid);
        // }
        
        [Fact]
        public void TestMutatePropRefStruct()
        {
            var guid = Guid.NewGuid();
            var test = new TestStruct(13)
            {
                Name = "Original",
                Guid = guid,
            };
            Assert(() => test.Id == 13 && 
                         test.Name == "Original" &&
                         test.Guid == guid);
            // This is using ref as should be
            MutatePropRef(ref test);
            Assert(() => test.Id == 147 && 
                         test.Name == "Mutated" &&
                         test.Guid != guid);
        }
        
        [Fact]
        public void TestMutatePropOutStruct()
        {
            var guid = Guid.NewGuid();
            var test = new TestStruct(13)
            {
                Name = "Original",
                Guid = guid,
            };
            Assert(() => test.Id == 13 && 
                         test.Name == "Original" &&
                         test.Guid == guid);
            // This is using ref as should be
            MutatePropOutForce(out test);
            Assert(() => test.Id == 147 && 
                         test.Name == "Mutated" &&
                         test.Guid == guid);
        }
        #endregion

        [Fact]
        public void TestRefFromObject()
        {
            Point originalPoint = new Point(3, 3);
            object boxed = (object) originalPoint;
            ref object refBox = ref boxed;
            ref Point refPoint = ref Danger.UnboxToRef<Point>(boxed);
            Debug.Assert(refPoint.X == originalPoint.X &&
                         refPoint.Y == originalPoint.Y);
            refPoint.X++;
            refPoint.Y++;
            Debug.Assert(refPoint.X == originalPoint.X + 1 &&
                         refPoint.Y == originalPoint.Y + 1);

        }
        
        
        
        public static ref T AsRef<T>(ref object boxedValue)
            where T : struct
        {
            Ldarg(nameof(boxedValue));
            Ldind_Ref();
            Emit.Unbox<T>();
            return ref ReturnRef<T>();
        }
    }
}*/