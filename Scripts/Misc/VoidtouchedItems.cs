using System.Collections;
using UnityEngine;
using HarmonyLib;

namespace Oddments
{
    [HarmonyPatch]
    public static class VoidtouchedItems
    {
        /*[HarmonyPatch(typeof(UINotificationController), nameof(UINotificationController.DoNotificationInternal))]
        [HarmonyPrefix]*/
        public static bool DoCorruptingNotification(UINotificationController __instance, NotificationParams notifyParams)
        {
            __instance.m_queuedNotifications.Add(__instance.HandleCorruptingNotification(notifyParams, notifyParams));
            __instance.m_queuedNotificationParams.Add(notifyParams);
            __instance.StartCoroutine(__instance.PruneQueuedNotifications());
            return false;
        }

        private static IEnumerator HandleCorruptingNotification(this UINotificationController self, NotificationParams notifyParams, NotificationParams corruptParam)
        {
            yield return null;
            self.SetupSprite(notifyParams.SpriteCollection, notifyParams.SpriteID);
            self.DescriptionLabel.ProcessMarkup = true;
            self.DescriptionLabel.ColorizeSymbols = true;
            self.NameLabel.Text = notifyParams.PrimaryTitleString.ToUpperInvariant();
            self.DescriptionLabel.Text = notifyParams.SecondaryDescriptionString;
            self.CenterLabel.Opacity = 1f;
            self.NameLabel.Opacity = 1f;
            self.DescriptionLabel.Opacity = 1f;
            self.CenterLabel.IsVisible = false;
            self.NameLabel.IsVisible = true;
            self.DescriptionLabel.IsVisible = true;
            /*SpriteOutlineManager.ToggleOutlineRenderers(self.notificationSynergySprite, false);
            SpriteOutlineManager.ToggleOutlineRenderers(self.notificationObjectSprite, false);*/
            dfSpriteAnimation component = self.BoxSprite.GetComponent<dfSpriteAnimation>();
            component.Stop();
            dfSpriteAnimation component2 = self.CrosshairSprite.GetComponent<dfSpriteAnimation>();
            component2.Stop();
            dfSpriteAnimation component3 = self.ObjectBoxSprite.GetComponent<dfSpriteAnimation>();
            component3.Stop();
            UINotificationController.NotificationColor forcedColor = notifyParams.forcedColor;
            string trackableGuid = notifyParams.EncounterGuid;
            bool isGold = forcedColor == UINotificationController.NotificationColor.GOLD || (!string.IsNullOrEmpty(trackableGuid) && GameStatsManager.Instance.QueryEncounterable(trackableGuid) == 1);
            bool isPurple = forcedColor == UINotificationController.NotificationColor.PURPLE || (!string.IsNullOrEmpty(trackableGuid) && EncounterDatabase.GetEntry(trackableGuid).usesPurpleNotifications);
            self.ToggleGoldStatus(isGold);
            self.TogglePurpleStatus(isPurple);
            bool singleLineSansSprite = notifyParams.isSingleLine;
            if (singleLineSansSprite || notifyParams.SpriteCollection == null)
            {
                self.ObjectBoxSprite.IsVisible = false;
                self.StickerSprite.IsVisible = false;
            }
            if (singleLineSansSprite)
            {
                self.CenterLabel.IsVisible = true;
                self.NameLabel.IsVisible = false;
                self.DescriptionLabel.IsVisible = false;
                self.CenterLabel.Text = self.NameLabel.Text;
            }
            else
            {
                self.NameLabel.IsVisible = true;
                self.DescriptionLabel.IsVisible = true;
                self.CenterLabel.IsVisible = false;
            }
            self.m_doingNotification = true;
            self.m_panel.IsVisible = false;
            GameUIRoot.Instance.MoveNonCoreGroupOnscreen(self.m_panel, false);
            float elapsed = 0f;
            float duration = 3f;
            bool hasPlayedAnim = false;
            if (singleLineSansSprite)
            {
                self.notificationObjectSprite.renderer.enabled = false;
                SpriteOutlineManager.ToggleOutlineRenderers(self.notificationObjectSprite, false);
            }
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                if (!hasPlayedAnim && elapsed > 0.75f)
                {
                    self.BoxSprite.GetComponent<dfSpriteAnimation>().Clip = ((!isPurple) ? ((!isGold) ? self.SilverAnimClip : self.GoldAnimClip) : self.PurpleAnimClip);
                    hasPlayedAnim = true;
                    self.ObjectBoxSprite.Parent.GetComponent<dfSpriteAnimation>().Play();
                }
                yield return null;
                self.m_panel.IsVisible = true;
                if (!singleLineSansSprite && notifyParams.SpriteCollection != null)
                {
                    self.notificationObjectSprite.renderer.enabled = true;
                    //SpriteOutlineManager.ToggleOutlineRenderers(self.notificationObjectSprite, true);
                }
            }
            self.SetupSynergySprite(corruptParam.SpriteCollection, corruptParam.SpriteID);
            elapsed = 0f;
            duration = 4f;
            self.notificationSynergySprite.renderer.enabled = true;
            dfSpriteAnimation boxSpriteAnimator = self.BoxSprite.GetComponent<dfSpriteAnimation>();
            boxSpriteAnimator.Clip = self.SynergyTransformClip;
            boxSpriteAnimator.Play();
            dfSpriteAnimation crosshairSpriteAnimator = self.CrosshairSprite.GetComponent<dfSpriteAnimation>();
            crosshairSpriteAnimator.Clip = self.SynergyCrosshairTransformClip;
            crosshairSpriteAnimator.Play();
            dfSpriteAnimation objectSpriteAnimator = self.ObjectBoxSprite.GetComponent<dfSpriteAnimation>();
            objectSpriteAnimator.Clip = self.SynergyBoxTransformClip;
            objectSpriteAnimator.Play();

            bool hasChangedName = false;
            while (elapsed < duration)
            {
                float baseSpriteLocalX = self.notificationObjectSprite.transform.localPosition.x;
                float synSpriteLocalX = self.notificationSynergySprite.transform.localPosition.x;
                self.CrosshairSprite.Size = self.CrosshairSprite.SpriteInfo.sizeInPixels * 3f;
                float p2u = self.BoxSprite.PixelsToUnits();
                Vector3 endPosition = self.ObjectBoxSprite.GetCenter();
                Vector3 startPosition = endPosition + new Vector3(0f, -120f * p2u, 0f);
                Vector3 startPosition2 = endPosition;
                Vector3 endPosition2 = endPosition + new Vector3(0f, 120f * p2u, 0f);
                //endPosition -= new Vector3(0f, 21f * p2u, 0f);
                float quickT = elapsed / 1f;
                float smoothT = Mathf.SmoothStep(0f, 1f, quickT);

                float num = Mathf.SmoothStep(0f, 1f, elapsed) * 2.5f;
                SpriteOutlineManager.ToggleOutlineRenderers(self.notificationObjectSprite, false);
                if (num < 1f)
                {
                    float realOpac = 1f - num;
                    self.NameLabel.Opacity = realOpac;
                    self.DescriptionLabel.Opacity = realOpac;

                    //self.notificationObjectSprite.usesOverrideMaterial = true;
                    //self.SetOutlineOpacityToo(1f - num);
                    self.notificationObjectSprite.color = new Color(realOpac, realOpac, realOpac, realOpac);
                    self.notificationSynergySprite.renderer.enabled = false;
                } else if (num > 1.5f)
                {
                    if (!hasChangedName)
                    {
                        hasChangedName = true;
                        self.NameLabel.text = corruptParam.PrimaryTitleString.ToUpperInvariant();
                        self.DescriptionLabel.text = corruptParam.SecondaryDescriptionString;
                    }
                    self.NameLabel.Opacity = -1.5f + num;
                    self.DescriptionLabel.Opacity = -1.5f + num;

                    //self.notificationSynergySprite.usesOverrideMaterial = true;
                    self.notificationSynergySprite.color = new Color(-1.5f + num, -1.5f + num, -1.5f + num, -1.5f + num);
                    self.notificationSynergySprite.renderer.enabled = true;

                    /*if (num >= 2.5f)
                    {
                        SpriteOutlineManager.ToggleOutlineRenderers(self.notificationSynergySprite, true);
                    }*/
                }

                Vector3 t2 = Vector3.Lerp(startPosition, endPosition, smoothT).Quantize(p2u * 3f).WithX(startPosition.x);
                Vector3 t3 = Vector3.Lerp(startPosition2, endPosition2, smoothT).Quantize(p2u * 3f).WithX(startPosition2.x);
                t3.y = Mathf.Max(startPosition2.y, t3.y);
                self.notificationSynergySprite.PlaceAtPositionByAnchor(t2, tk2dBaseSprite.Anchor.MiddleCenter);
                self.notificationSynergySprite.transform.position = self.notificationSynergySprite.transform.position + new Vector3(0f, 0f, -0.125f);
                self.notificationObjectSprite.PlaceAtPositionByAnchor(t3, tk2dBaseSprite.Anchor.MiddleCenter);
                self.notificationObjectSprite.transform.localPosition = self.notificationObjectSprite.transform.localPosition.WithX(baseSpriteLocalX);
                self.notificationSynergySprite.transform.localPosition = self.notificationSynergySprite.transform.localPosition.WithX(synSpriteLocalX);
                self.notificationSynergySprite.UpdateZDepth();
                self.notificationObjectSprite.UpdateZDepth();
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            GameUIRoot.Instance.MoveNonCoreGroupOnscreen(self.m_panel, true);
            elapsed = 0f;
            duration = 0.25f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }

            //self.SetOutlineOpacityToo(1f);
            self.notificationObjectSprite.color = Color.white;
            self.CenterLabel.Opacity = 1f;
            self.NameLabel.Opacity = 1f;
            self.DescriptionLabel.Opacity = 1f;
            self.CenterLabel.IsVisible = false;
            self.NameLabel.IsVisible = true;
            self.DescriptionLabel.IsVisible = true;
            self.DisableRenderers();
            self.m_doingNotification = false;
            yield break;
        }

        /*public static void SetOutlineOpacityToo(this UINotificationController self, float opacity)
        {
            for (int i = 0; i < self.outlineSprites.Length; i++)
            {
                self.outlineSprites[i].color = self.outlineSprites[1].color.WithAlpha(opacity);
            }
        }*/
    }
}
