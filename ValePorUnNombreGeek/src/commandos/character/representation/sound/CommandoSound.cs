using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.representation.sound
{
    class CommandoSound
    {
        private TgcStaticSound[] okSounds = new TgcStaticSound[] {};
        private TgcStaticSound[] shotSounds = new TgcStaticSound[] {};
        private TgcStaticSound[] doneSounds = new TgcStaticSound[] {};
        private TgcStaticSound[] errorSounds = new TgcStaticSound[] {};
        private TgcStaticSound lastPlayed;
        private static Random random = new Random(); // OMG

        private static TgcStaticSound soundFor(String fileName)
        {
            TgcStaticSound sound = new TgcStaticSound();
            sound.loadSound(CommandosUI.Instance.MediaDir + "Sounds\\" + fileName);
            return sound;
        }

        internal static CommandoSound commando()
        {
            if (random.Next(0, 2) == 0)
            {
                return greenBerret();
            }
            else
            {
                return zapper();
            }
        }

        internal static CommandoSound greenBerret()
        {
            CommandoSound instance = new CommandoSound();
            instance.okSounds = new[] { soundFor("berret-coming-over.wav"), soundFor("berret-command.wav"), soundFor("berret-consider-it-done.wav") };
            instance.shotSounds = new[] { soundFor("berret-shot-ah.wav"), soundFor("berret-shot-get-me-out-of-here.wav"), soundFor("berret-shot-that-hurts.wav"), soundFor("berret-shot-uh.wav")};
            instance.doneSounds = new [] { soundFor("berret-im-coming.wav"), soundFor("berret-just-leave-it-to-me.wav"), soundFor("berret-ok.wav"), soundFor("berret-thats-easy.wav") };
            instance.errorSounds = new[] { soundFor("berret-are-you-crazy.wav"), soundFor("berret-he-wish-i-could-do-that.wav"), soundFor("berret-i-cant.wav") };
            return instance;
        }

        internal static CommandoSound zapper()
        {
            CommandoSound instance = new CommandoSound();
            instance.okSounds = new[] { soundFor("zapper-affirmative-sir.wav"), soundFor("zapper-ill-be-there-in-a-moment.wav"), soundFor("zapper-ill-be-right-over.wav"), soundFor("zapper-ill-be-there-in-a-moment.wav") };
            instance.shotSounds = new[] { soundFor("zapper-shot-ah-sir-i-need-help.wav"), soundFor("zapper-shot-ah.wav"), soundFor("zapper-shot-im-moolding.wav"), soundFor("zapper-shot-sir-please.wav") };
            instance.doneSounds = new[] { soundFor("zapper-fine-sir.wav"), soundFor("zapper-ok-sir.wav"), soundFor("zapper-yes-sir.wav") };
            instance.errorSounds = new[] { soundFor("zapper-i-cant-do-that-sir.wav"), soundFor("zapper-im-afraid-that-is-not-possible.wav"), soundFor("zapper-negative-sir.wav"), soundFor("zapper-understood-sir.wav") };
            return instance;
        }

        internal static CommandoSound soldier()
        {
            CommandoSound instance = new CommandoSound();
            instance.shotSounds = new[] { soundFor("knife.wav") };
            return instance;
        }

        private CommandoSound() { }

        internal void dispose()
        {
            foreach(TgcStaticSound sound in okSounds.Concat(shotSounds).Concat(doneSounds).Concat(errorSounds)) {
                sound.dispose();
            }
        }

        internal void ok()
        {
            this.play(okSounds);
        }

        internal void shot()
        {
            this.play(shotSounds);
        }

        internal void done()
        {
            this.play(doneSounds);
        }

        internal void error()
        {
            this.play(errorSounds);
        }

        private void play(TgcStaticSound[] sounds)
        {
            if (sounds.Length != 0)
            {
                /*if (lastPlayed == null || !lastPlayed.SoundBuffer.Status.Playing)
                {*/
                    lastPlayed = sounds[random.Next(0, sounds.Length)];
                    lastPlayed.play();
                //}
            }
        }
    }
}
