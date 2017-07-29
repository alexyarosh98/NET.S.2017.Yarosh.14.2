using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixes
{
    public abstract class Matrixes<T>
    {
        private int dimension;
        protected internal T[,] matrix;

        public int Dimension
        {
            get { return dimension; }
        }

        public EventHandler ChangeHandler;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="n">dimmension of matrix</param>
        protected Matrixes(int n)
        {
            if (n <= 0) throw new ArgumentOutOfRangeException($"{nameof(n)} must be a positive number");

            dimension = n;
            matrix = new T[dimension, dimension];
        }
        /// <summary>
        /// Change element on postition (i,j)
        /// </summary>
        /// <param name="i">line postition of element to be chanched</param>
        /// <param name="j">column position of element to be changed</param>
        /// <param name="obj">new element</param>
        /// <exception cref="ArgumentOutOfRangeException">i and j must be greater then 0 but less then dimension of matrix</exception>
        /// <exception cref="ArgumentNullException">new element must not be null</exception>
        public virtual void ChangeElement(int i, int j, T obj)
        {
            if(i<0||j<0||j>=Dimension||i>=Dimension) throw new ArgumentOutOfRangeException
                    ($"{nameof(i)} and {nameof(j)} must be from 0 to {Dimension-1}");
            if(ReferenceEquals(obj,null)) throw new ArgumentNullException($"{nameof(obj)} must not be null");

            matrix[i, j] = obj;

            MatrixChanged(this,new EventArgs());
        }

        #region Addition
        /// <summary>
        /// Add matrix to this one
        /// </summary>
        /// <param name="obj">matrix to be added</param>
        /// <param name="additionFunc">logic of addition 2 elements in matrixes</param>
        /// <exception cref="ArgumentNullException">parametres must not be null</exception>
        /// <exception cref="InvalidOperationException">type of elements in both matrixes must be the same</exception>
        /// <returns>this matrix(changed) if types are the same and new SquareMatrix if types are different</returns>
        public virtual Matrixes<T> AddMatrixes(Matrixes<T> obj, Func<T, T, T> additionFunc)
        {
            if (ReferenceEquals(obj, null)) throw new ArgumentNullException($"{nameof(obj)} must not be null");
            if(ReferenceEquals(additionFunc,null)) throw new ArgumentNullException($"{nameof(additionFunc)} must not be null");
            if (matrix.GetType() != obj.matrix.GetType())
                throw new InvalidOperationException("Type of elements in matrixes must be the same");


            if (this.GetType() == obj.GetType())
            {
                this.AddMatrix(obj, additionFunc);
                return this;
            }

            SquareMatrix<T> newMatrix = new SquareMatrix<T>(Math.Max(obj.Dimension, Dimension));
            for (int i = 0; i < newMatrix.Dimension; i++)
            {
                for (int j = 0; j < newMatrix.Dimension; j++)
                {
                    if (this.Dimension >= obj.Dimension)
                    {
                        newMatrix.matrix[i, j] = matrix[i, j];
                    }
                    else
                    {
                        newMatrix.matrix[i, j] = obj.matrix[i, j];
                    }
                }
            }

            if (this.Dimension >= obj.Dimension)
            {
                newMatrix.AddMatrix(obj, additionFunc);
            }
            else
            {
                newMatrix.AddMatrix(this, additionFunc);
            }

            return newMatrix;

        }
        private  void AddMatrix(Matrixes<T> obj, Func<T, T, T>additionFunc)
        {
            if (ReferenceEquals(obj, null)) throw new ArgumentNullException($"{nameof(obj)} must not be null");
            if (matrix.GetType() != obj.matrix.GetType())
                throw new InvalidOperationException("Type of elements in matrixes must be the same");
            
            int maxDim = Math.Max(obj.Dimension, Dimension);
            int minDim = Math.Min(obj.Dimension, Dimension);

            T[,] newMatrix = new T[maxDim, maxDim];

            for (int i = 0; i < minDim; i++)
            {
                for (int j = 0; j < minDim; j++)
                {
                    newMatrix[i, j] = additionFunc(matrix[i, j], obj.matrix[i, j]);
                }
            }

            T[,] arr = null;
            if (obj.Dimension > Dimension) arr = obj.matrix;
            else if (obj.Dimension < Dimension) arr = this.matrix;

            if (arr != null)
            {
                for (int i = 0; i < maxDim; i++)
                {
                    for (int j = minDim; j < maxDim; j++)
                    {
                        newMatrix[i, j] = arr[i, j];
                    }
                }
                for (int i = minDim; i < maxDim; i++)
                {
                    for (int j = 0; j < maxDim; j++)
                    {
                        newMatrix[i, j] = arr[i, j];
                    }
                }
            }

            matrix = newMatrix;
            dimension = maxDim;
        }



        #endregion

        private void MatrixChanged(object o, EventArgs args)
        {
            ChangeHandler(o, args);
        }
    }
}
