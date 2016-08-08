TextureMatchChecker
	功能：
		1.根据选择文件目录检索所有的PNG文件 （Sprite）
			首先将需要检测的文件目录都拖进界面中，然后点击“搜索Sprite”
		2.根据选择文件目录检索所有包含了Sprite资源的Prefab文件
			首先将需要检测的文件目录都拖进界面中，然后点击“搜索Prefab”
			
		3.当点击了“搜索Sprite”并且检索到Sprite后，可以点击“匹配Sprite”检索全部的Sprite是否
			有出现内容相同的情况，如果有，则在列表中显示
		4.当“匹配Sprite”与“搜索Prefab”后，可以点击“关联Sprite与Prefab”按钮，此时将根据Prefab
			中的Sprite中的资源ID于Sprite列表匹配，刷新Sprite资源的引用数
		5.当关联了文件之后，可以选择“删除无引用Sprite” ，将会将所有引用数为0的Sprite资源删除掉（需要刷子Assert文件）
		6.选择“剔除重复Sprite”按钮，将会把所有重复的Sprite文件删除剩下一个，并且强制将Prefab中的引用修改。
	
			
			
			