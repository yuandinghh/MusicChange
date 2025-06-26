# MusicChange

# MusicChange is a music player that can change the music speed and pitch.
# It is built using Python and 

# 1. 查看当前远程仓库配置（确认远程名称，一般默认是 origin ）
git remote -v  
# 2. 修改远程仓库地址（替换为新的仓库 URL ）
git remote set-url origin 新仓库地址  
# 3. 验证是否修改成功  
git remote -v  

git init
git add README.md
git commit -m "first commit"
git branch -M main
git remote add origin https://github.com/yuandinghh/MusicChange.git
git push -u origin main
Tkinter for the GUI, and Pydub for audio processing.
# Features
# - Change music speed
# - Change music pitch