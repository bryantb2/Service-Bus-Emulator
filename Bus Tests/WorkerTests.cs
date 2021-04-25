using System;
using Xunit;
using ServiceBus;

namespace Bus_Tests
{
    public class WorkerTests
    {
        // setup fields
        private Action<BusEvent<int>> proccessingFunc;
        private Action<BusEvent<int>> cleanupFunc;
        private Worker<int> processOnlyWorker;
        private Worker<int> cleanupOnlyWorker;
        private Worker<int> processAndCleanupWorker;
        private BusEvent<int> busEvent;

        public WorkerTests()
        {
            // action definitions
            this.proccessingFunc = (e) =>
            {
                // process logic
                e.Payload = e.Payload + 20;
            };
            this.cleanupFunc = (e) =>
            {
                // cleanup logic
                e.Payload = e.Payload - 100;
                this.busEvent.Processed = true;
            };

            // workers
            this.processOnlyWorker = new Worker<int>(this.proccessingFunc,
                (e) => { }
            );
            this.cleanupOnlyWorker = new Worker<int>(
                (e) => { },
                this.cleanupFunc
            );
            this.processAndCleanupWorker = new Worker<int>(this.proccessingFunc, this.cleanupFunc);

            // bus event
            this.busEvent = new BusEvent<int>()
            {
                EventId = 2,
                Processing = false,
                Processed = false,
                CreationTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Payload = 10
            };
        }

        [Fact]
        [Trait("Payload Processing", "Process")]
        public async void Processing()
        {
            // act
            await this.processOnlyWorker.ProcessEvent(this.busEvent);

            // assert
            Assert.Equal(30, this.busEvent.Payload);
        }

        [Fact]
        [Trait("Payload Processing", "Cleanup")]
        public async void Cleanup()
        {
            // act
            await this.cleanupOnlyWorker.ProcessEvent(this.busEvent);

            // assert
            Assert.Equal(-90, this.busEvent.Payload);
        }

        [Fact]
        [Trait("Payload Processing", "Process and Cleanup")]
        public async void ProcessingAndCleanup()
        {
            // act
            await this.processAndCleanupWorker.ProcessEvent(this.busEvent);

            // assert
            Assert.Equal(-70, this.busEvent.Payload);
        }

        [Fact]
        [Trait("Payload Processing", "Error")]
        public void ErrorHandling()
        {
            // arrange
            var errorThrowingWorker = new Worker<int>(
                (e) =>
                {
                    // hrow processing error
                    throw new Exception("Error throw as part of payload error handling test");
                },
                (e) => { }
            );

            // act
            Assert.ThrowsAsync<Exception>(async () => await errorThrowingWorker.ProcessEvent(this.busEvent));
        }
    }
}
