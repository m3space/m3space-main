using System.Collections;
using System.Threading;

namespace M3Space.Capsule
{
    public class BoundedBuffer
    {
        private Queue m_queue = new Queue();
        private int m_consumersWaiting;
        private int m_producersWaiting;
        private const int m_maxBufferSize = 128;
        private AutoResetEvent m_are = new AutoResetEvent(false);

        public int Count
        {
            get { return m_queue.Count; }
        }

        public void Add(object obj)
        {
            Monitor.Enter(m_queue);
            try
            {
                while (m_queue.Count == (m_maxBufferSize - 1))
                {
                    m_producersWaiting++;
                    Monitor.Exit(m_queue);
                    m_are.WaitOne();
                    Monitor.Enter(m_queue);
                    m_producersWaiting--;
                }
                m_queue.Enqueue(obj);
                if (m_consumersWaiting > 0)
                    m_are.Set();
            }
            finally
            {
                Monitor.Exit(m_queue);
            }
        }

        public object Take()
        {
            object item;
            Monitor.Enter(m_queue);
            try
            {
                while (m_queue.Count == 0)
                {
                    m_consumersWaiting++;
                    Monitor.Exit(m_queue);
                    m_are.WaitOne();
                    Monitor.Enter(m_queue);
                    m_consumersWaiting--;
                }
                item = m_queue.Dequeue();
                if (m_producersWaiting > 0)
                    m_are.Set();
            }
            finally
            {
                Monitor.Exit(m_queue);
            }
            return item;
        }
    }
}
