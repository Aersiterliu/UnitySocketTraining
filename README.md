# UnitySocketTraining
**关于一些学到的新知识**
解决沾包一般有三种方法
**1:长度信息法:**
在每个数据包前加入长度信息.每次接收到数据后,先读取表示长度的字节.
如果缓冲区的数据长度大于要取出的字节数,则取出.否则等待下一次数据接收
**核心思想:定义一个缓冲区readBuff和一个指示缓冲区有效数据长度变量buffCount**
**2:固定长度法:**
每次都以相同长度发送数据
**3:结束符号法**:
规定一个结束符号,作为消息符的分隔

00000000
8位为一个字节大小
16位为两个字节大小

`Int16 bodyLength=BitConverter.ToInt16(readBuff,0);`
表示取缓冲区readBuff的某个字节开始(这里是0,表示从第一个字节开始)的2个字节(因为Int16需要两个字节表示)数据,再把它转换成数字
