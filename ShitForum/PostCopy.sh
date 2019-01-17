
sudo 7z x publish.7z -o/home/Forum/ShitForum
sudo chown -R www-data:www-data /home/Forum/

sudo mv /home/Forum/ShitForm/kestrel-shitforum.service /etc/systemd/system/kestrel-shitforum.service
systemctl enable kestrel-shitforum.service
systemctl start kestrel-shitforum.service
systemctl status kestrel-shitforum.service
