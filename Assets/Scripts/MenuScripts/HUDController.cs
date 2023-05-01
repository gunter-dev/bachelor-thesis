using UnityEngine;
using UnityEngine.UI;

namespace MenuScripts
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private Image speed;
        [SerializeField] private Image jump;
        [SerializeField] private Image shield;
        [SerializeField] private Image nightVision;

        private Color _active;
        private Color _inactive;

        private Player _player;

        private Constants.PlayerSkills _activeSkillImage;

        private void Start()
        {
            _active = new Color(1, 1, 1, Constants.ActiveTransparency);
            _inactive = new Color(1, 1, 1, Constants.InactiveTransparency);
        }

        private void Update()
        {
            if (!_player)
            {
                _player = GameObject.FindGameObjectWithTag(Constants.PlayerTag).GetComponent<Player>();
                SkillChanged(_player.activeSkill);
            }
            else
            {
                Constants.PlayerSkills skill = _player.activeSkill;
                if (skill != _activeSkillImage) SkillChanged(skill);
            }
        }

        private void SkillChanged(Constants.PlayerSkills skill)
        {
            speed.color = skill == Constants.PlayerSkills.HighSpeed ? _active : _inactive;
            jump.color = skill == Constants.PlayerSkills.HighJump ? _active : _inactive;
            shield.color = skill == Constants.PlayerSkills.Unbreakable ? _active : _inactive;
            nightVision.color = skill == Constants.PlayerSkills.NightVision ? _active : _inactive;

            _activeSkillImage = skill;
        }
    }
}
