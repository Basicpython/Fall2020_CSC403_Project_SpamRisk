﻿using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.Media;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project {
  public partial class FrmBattle : Form {
    public static FrmBattle instance = null;
    private EnemyType enemy;
    private Player player;
    Random rnd = new Random();
    string newLine = Environment.NewLine;
    string foeName = "";


        private FrmBattle() {
      InitializeComponent();
      player = Game.player;
          
            this.FormClosed += (s, args) =>
            {
                instance = null;
                enemy.AttackEvent -= PlayerDamage;
                enemy.HealEvent -= EnemyHeal;
                player.AttackEvent -= EnemyDamage;
                player.HealEvent -= PlayerHeal;
                enemy.resetEnemyClass();
            };

    }
    
    public void Setup() {
      // update for this enemy
      battleTheme.PlayLooping();
      picEnemy.BackgroundImage = enemy.Img;
      picEnemy.Refresh();
      foeName = enemy.Name;
      BackColor = enemy.Color;
      picBossBattle.Visible = false;
      enemy.setEnemyClass();
      enemy.setClassHealth();

      // Observer pattern
      enemy.AttackEvent += PlayerDamage;
      enemy.HealEvent += EnemyHeal;
      player.AttackEvent += EnemyDamage;
      player.HealEvent += PlayerHeal;

      // show health
      UpdateHealthBars();
    }

    public void SetupForBossBattle() {
      picBossBattle.Location = Point.Empty;
      picBossBattle.Size = ClientSize;
      picBossBattle.Visible = true;
      //SoundPlayer simpleSound = new SoundPlayer(Resources.final_battle);
      //simpleSound.Play();
      battleTheme.PlayLooping();
      enemy.setBossClass(4);
      enemy.setClassHealth();
      tmrFinalBattle.Enabled = true;
      UpdateHealthBars();
     }

    public static FrmBattle GetInstance(EnemyType enemy) {
      if (instance == null) {
        instance = new FrmBattle();
        instance.enemy = enemy;
        instance.Setup();
       }
      return instance;
    }

        private void UpdateHealthBars() {
      if (player.Health > player.MaxHealth)
            {
                player.AlterHealth(player.MaxHealth - player.Health);
            }
      if (enemy.Health > enemy.MaxHealth)
            {
                enemy.setClassHealth();
            }
      float playerHealthPer = player.Health / (float)player.MaxHealth;
      float enemyHealthPer = enemy.Health / (float)enemy.MaxHealth;

      const int MAX_HEALTHBAR_WIDTH = 226;
      lblPlayerHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * playerHealthPer);
      lblEnemyHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * enemyHealthPer);
      lblPlayerHealthFull.Text = player.Health.ToString();
      lblEnemyHealthFull.Text = enemy.Health.ToString();
    }

    private void btnAttack_Click(object sender, EventArgs e) {
      int attack = rnd.Next(-5, -2);
      string log = string.Format("You raise your ancestral staff of nut and thwap the foe dealing {0} damage", -(attack * 2));
      battleLog.AppendText(log);
      battleLog.AppendText(newLine);
      player.OnAttack(attack);
      battleTheme.Stop();
      attackSound.PlaySync();
      if (enemy.Health > 0) {
       log = (foeName + enemy.determineAttack(0));
       battleLog.AppendText(log);
       battleLog.AppendText(newLine);
       enemy_attack.PlaySync();

        PlayerScore(10);
      }

      UpdateHealthBars();
      battleTheme.PlayLooping();

      if (enemy.Health <= 0) {
        instance = null;
        Close();
      } else if (player.Health == 0)
            {
                instance = null;
                Close();
            }
    }

    private void btnHeal_Click(object sender, EventArgs e)
    {
        int heal = 0;
        if (player.Health <= 0 || enemy.Health <= 0)
        {
            instance = null;
            Close();
        }
        else
        {

                if ((player.Health + 8) > 30)
                {
                    heal = (30 - player.Health);
                    string log = string.Format("You betray your nut family, devouring them for {0} health", (heal));
                    battleLog.AppendText(log);
                    battleLog.AppendText(newLine);
                    battleTheme.Stop();
                    healSound.PlaySync();
                    player.OnHeal(heal);
                }
                else
                {
                    heal = 8;
                    string log = string.Format("You betray your nut family, devouring them for {0} health", (heal));
                    battleLog.AppendText(log);
                    battleLog.AppendText(newLine);
                    battleTheme.Stop();
                    healSound.PlaySync();
                    player.OnHeal(heal);
                }


                if (enemy.Health > 0)
                {
                    string log = (foeName + enemy.determineAttack(0));
                    battleLog.AppendText(log);
                    battleLog.AppendText(newLine);
                    enemy_attack.PlaySync();
                }
                battleTheme.PlayLooping();
                UpdateHealthBars();
        }
    }

        private void btnDodge_Click(object sender, EventArgs e)
        {
            if (player.Health <= 0 || enemy.Health <= 0)
            {
                instance = null;
                Close();
            }
            else
            {
                string log =  "You perform some of the sickest acrobatics ever performed by a mortal to distract and awe your foe";
                battleLog.AppendText(log);
                battleLog.AppendText(newLine);
                log = (foeName + enemy.determineAttack(1));
                battleLog.AppendText(log);
                battleLog.AppendText(newLine);
                enemy_attack.PlaySync();
                dodgeSound.PlaySync();
                battleTheme.PlayLooping();
                UpdateHealthBars();
            }
        }
    private void btnFlee_Click(object sender, EventArgs e)
        {
            string log = "You were a complete and utter buffoon to attempt this fight. You salvage what little pride you still have and walk away";
            battleLog.AppendText(log);
            battleLog.AppendText(newLine);
            SoundPlayer fleeSound = new SoundPlayer(Resources.flee);
            fleeSound.PlaySync();
            //observers have to be cleared, otherwise other instances will do n*damage
            enemy.AttackEvent -= PlayerDamage;
            player.AttackEvent -= EnemyDamage;
            player.HealEvent -= PlayerHeal;
            instance = null;
            Close();

        }

    private void EnemyDamage(int amount) {
      enemy.AlterHealth(amount);
    }

    private void PlayerDamage(int amount) {
      player.AlterHealth(amount);
    }

    private void PlayerScore(int amount)
    {
        player.AlterScore(amount);
        lblPlayerScore.Text = "Score: " + player.Score.ToString();
        }

        private void PlayerHeal(int amount)
        {
            player.AlterHealth(amount);
        }

    private void EnemyHeal(int amount)
        {
            enemy.AlterHealth(amount);
        }

    private void tmrFinalBattle_Tick(object sender, EventArgs e) {
      picBossBattle.Visible = false;
      tmrFinalBattle.Enabled = false;
    }
  }
}
