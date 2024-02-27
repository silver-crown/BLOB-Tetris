using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Monogame_Tetris
{
    public class SoundManager
    {
        private ContentManager _contentManager;
        public Song tetrisMusic;
        public SoundEffect drOakWonderful;
        public SoundEffect drOakWellDone;
        public SoundEffect drOakPerfect;
        public SoundEffect ZA_WARUDO;

        public SoundManager(ContentManager c) {
            _contentManager = c;
        }
        public void LoadContent() {
            tetrisMusic = _contentManager.Load<Song>("tetris music");
            drOakWonderful = _contentManager.Load<SoundEffect>("drOakWonderful");
            drOakWellDone = _contentManager.Load<SoundEffect>("drOakwell-done");
            drOakPerfect = _contentManager.Load<SoundEffect>("drOakPerfect");
            ZA_WARUDO = _contentManager.Load<SoundEffect>("ZA_WARUDO");
        }

        public void PlaySong(Song song, float volume, bool isRepeating) {
            MediaPlayer.Play(song);
            MediaPlayer.Volume = volume; // Set the volume to 50%
            MediaPlayer.IsRepeating = isRepeating; // Set to true if you want the music to loop
        }
        public void PlaySoundEffect(SoundEffect soundEffect) {
            soundEffect.Play(volume: 1f, pitch: 0.0f, pan: 0.0f);
        }
        public void StopSong() {
            MediaPlayer.Stop();
        }
    }
}
