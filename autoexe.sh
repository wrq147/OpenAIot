#!/bin/bash

export DOTNET_ROOT=/root/.dotnet
export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools

# 先清理旧进程，避免重复启动
pkill -f App
pkill -f MqttChannel


cd /root/card
chmod +x App
./App > app.log 2>&1 &
sleep 2s
runrs=true
while [ "$runrs" = true ];
do 
 sleep 3s
 log_file_path="./app.log"
 search_string="服务已启动"
 # 使用grep检查日志文件是否包含指定字符串
 grep -q "$search_string" "$log_file_path"
 if [ $? -eq 0 ]; then
   echo "Start Successfully"
   cd /root/channel
   chmod +x MqttChannel
   sleep 1s
   ./MqttChannel &
   runrs=false
 else
   echo "Start failed"
   pgrep -f App
   if [ $? -eq 0 ]; then
     echo "App Running"
   else
     ./App > app.log 2>&1 &
   fi
 fi	
done
