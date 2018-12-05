
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Movolira {
    class CardMovieDecoration : RecyclerView.ItemDecoration{
        public CardMovieDecoration(Context context) : base(){
            this.context = context;
        }
        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state) {
            float density = context.Resources.DisplayMetrics.Density;
            int offset = (int)(density * 7);
            int child_pos = parent.GetChildLayoutPosition(view);
            int child_count = state.ItemCount;
            Console.WriteLine(child_count);
            if (context.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape) {
                if (child_pos < 4) {
                    outRect.Top = 2 * offset;
                    outRect.Bottom = offset;
                } else if (child_count - 4 <= child_pos) {
                    outRect.Top = offset;
                    outRect.Bottom = 2 * offset;
                } else {
                    outRect.Top = offset;
                    outRect.Bottom = offset;
                }
                if (child_pos % 4 == 0) {
                    outRect.Left = 2 * offset;
                    outRect.Right = offset;
                } else if((child_pos + 1) % 4 == 0){
                    outRect.Left = offset;
                    outRect.Right = 2 * offset;
                } else {
                    outRect.Left = offset;
                    outRect.Right = offset;
                }
            } else {
                if (child_pos < 2) {
                    outRect.Top = 2 * offset;
                    outRect.Bottom = offset;
                } else if (child_count - 2 <= child_pos){
                    outRect.Top = offset;
                    outRect.Bottom = 2 * offset;
                } else {
                    outRect.Top = offset;
                    outRect.Bottom = offset;
                } 
                if (child_pos % 2 == 0) {
                    outRect.Left = 2 * offset;
                    outRect.Right = offset;
                } else {
                    outRect.Left = offset;
                    outRect.Right = 2 * offset;
                }
            }
        }
        private Context context;
    }
}