# on client
ssh-keygen -t rsa -b 4096 -C "email@domain.com"
scp ~/.ssh/id_rsa.pub user@server.example.com:~/.ssh/authorized_keys
rm -f ~/.ssh/id_rsa.pub # not needed anymore

# on server, http://man.he.net/man5/sshd_config
cd /etc/ssh
sudo vim sshd_config

# apply the following configuration options
# /etc/ssh/sshd_config
PasswordAuthentication no
ChallengeResponseAuthentication no
UsePAM no