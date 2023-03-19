using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class VolleySpamPlayerItem : PlayerItem
    {
        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            StartCoroutine(Alexandria.ItemAPI.ItemBuilder.HandleDuration(this, Duration, user, null));
        }

        public IEnumerator FireVolleys()
        {
            yield break;
        }

        public ProjectileVolleyData BaseVolley;
        public float Duration; 
    }
}
