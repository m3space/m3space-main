using System;
using System.Collections;

namespace NMEA
{
    /// <summary>
    /// Класс, реализующий потокобезопасный FIFO буфер объектов
    /// </summary>
    public class SynchronizedObjectQueue
    {
        #region Properties

        /// <summary>
        /// Зарезервированный размер очереди по умолчанию
        /// </summary>
        public const int defaultCapacity = 32;
        /// <summary>
        /// Максимальный размер очереди по умолчанию
        /// </summary>
        public const int defaultMaxSize = 1024;

        int maxSize = defaultMaxSize;
        /// <summary>
        /// Максимальный размер очереди
        /// </summary>
        public int MaxSize
        {
            get { return maxSize; }
            set 
            {
                if ((value > 0) && (value < short.MaxValue))
                {
                    maxSize = value;
                }
                else
                {
                    throw new ArgumentException("Максимальный размер буфера должен лежать в диапазоне от 1 до " + short.MaxValue, "MaxSize");
                }

            }
        }

        Queue buffer;

        #endregion

        #region Constructor

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public SynchronizedObjectQueue()
            : this(defaultCapacity)
        {
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="capacity">Начальный размер</param>
        public SynchronizedObjectQueue(int capacity)
            : this(capacity, defaultMaxSize)
        {
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="capacitty">Начальный размер</param>
        /// <param name="maxCount">Максимальный размер</param>
        public SynchronizedObjectQueue(int capacitty, int maxCount)
        {
            buffer = Queue.Synchronized(new Queue(capacitty));
            maxSize = maxCount;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Возвращает текущее количество элементов в очереди
        /// </summary>
        public int Count
        {
            get
            {
                return buffer.Count;
            }
        }

        /// <summary>
        /// Очистка очереди
        /// </summary>
        public void Clear()
        {
            buffer.Clear();
        }

        /// <summary>
        /// Добавление элемента в очередь
        /// </summary>
        /// <param name="item">Искомый элемент</param>
        public void Add(object item)
        {
            buffer.Enqueue(item);

            if (buffer.Count >= maxSize)
            {
                if (QueueIverflow != null)
                {
                    QueueIverflow.BeginInvoke(buffer.Dequeue(), null, null);
                }
                else
                {
                    buffer.Dequeue();
                }
            }

            if (ElementAdded != null)
            {
                ElementAdded();
            }            
        }

        /// <summary>
        /// Выборка элемента из головы очереди и удаление его из очереди
        /// </summary>
        /// <returns>Начальный элемент очереди</returns>
        public object Dequeque()
        {
            return buffer.Dequeue();
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Тип делегата на событие добавления элемента в очередь
        /// </summary>
        public delegate void ElementAddedEventHandler();

        /// <summary>
        /// Событие добавление элемента в очередь
        /// </summary>
        public ElementAddedEventHandler ElementAdded;

        public delegate void QueueOverflowEventHandler(object queueHead);
        public QueueOverflowEventHandler QueueIverflow;

        #endregion
    }
}
