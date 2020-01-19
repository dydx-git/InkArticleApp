using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace InkArticleApp
{
    public class ProcessAnimatorTimer : ObservableObject
    {
        

        Random random = new Random();

        private string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private string _animatedWord;

        public string AnimatedWord
        {
            get { return _animatedWord; }
            set { Set(ref _animatedWord, value); }
        }

        DispatcherTimer AnimatorTimer;

        private int _duration;
        public int Duration
        {
            get { return _duration; }
            set { Set(ref _duration, value); }
        }

        public ProcessAnimatorTimer()
        {
            this.Duration = 200;
        }
        
        public void StartTimer()
        {
            AnimatorTimer = new DispatcherTimer();
            AnimatorTimer.Interval = TimeSpan.FromMilliseconds(this.Duration);
            AnimatorTimer.Tick += AnimatorTimer_Tick;
            AnimatorTimer.Start();
        }

        public void StopTimer()
        {
            if (AnimatorTimer != null)
            {
                AnimatorTimer.Stop();
            }
        }

        private void AnimatorTimer_Tick(object sender, object e)
        {
            AnimatorTimer.Stop();

            AnimatorTimer.Interval = TimeSpan.FromMilliseconds(random.Next(100));

            char[] alphabets = chars.ToCharArray();

            FisherYatesShuffler.Shuffle(alphabets);

            AnimatedWord = new String(alphabets.Take(random.Next(4, 8)).ToArray());

            AnimatorTimer.Start();
        }
    }
}
