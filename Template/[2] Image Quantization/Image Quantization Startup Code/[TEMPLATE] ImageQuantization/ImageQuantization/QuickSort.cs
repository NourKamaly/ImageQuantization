//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ImageQuantization
//{
//    class QuickSort<T>
//    {
//        public static int Partition(Edges[] arr, int left, int right)
//        {
//            double pivot;
//            pivot = arr[left].weight;
//            while (true)
//            {
//                while (arr[left] < pivot)
//                {
//                    left++;
//                }
//                while (arr[right] > pivot)
//                {
//                    right--;
//                }
//                if (left < right)
//                {
//                    int temp = arr[right];
//                    arr[right] = arr[left];
//                    arr[left] = temp;
//                }
//                else
//                {
//                    return right;
//                }
//            }
//        }
//        public static void quickSort(int[] arr, int left, int right)
//        {
//            int pivot;
//            if (left < right)
//            {
//                pivot = Partition(arr, left, right);
//                if (pivot > 1)
//                {
//                    quickSort(arr, left, pivot - 1);
//                }
//                if (pivot + 1 < right)
//                {
//                    quickSort(arr, pivot + 1, right);
//                }
//            }
//        }

//    }
//}
