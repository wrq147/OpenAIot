export const parts = [{
	name: '眼睛',
	path: 'eye/',
	x: 66,
	y: 96,
	width: 148,
	height: 58,
	imgs: [
		'Closed.png',
		'Cry.png',
		'Default.png',
		'Eye-Roll.png',
		'Happy.png',
		'Hearts.png',
		'Side.png',
		'Squint.png',
		'Surprised.png',
		'Wink-Wacky.png',
		'Wink.png',
		'X-Dizzy.png',
	]
},{
	name: '眉毛',
	path: 'eyebrow/',
	x: 66,
	y: 84,
	width: 148,
	height: 33,
	imgs: [
		'x',
		'Angry-Natural.png',
		'Default-Natural.png',
		'Flat-Natural.png',
		'Frown-Natural.png',
		'Raised-Excited-Natural.png',
		'Sad-Concerned-Natural.png',
		'Unibrow-Natural.png',
		'Up-Down-Natural.png'
	]
}, {
	name: '鼻子',
	hide: true,
	x: 103,
	y: 139,
	width: 74,
	height: 32,
	path: 'nose/',
	imgs: [
		'Default.png'
	]
}, {
	name: '嘴',
	path: 'mouth/',
	x: 69,
	y: 155,
	width: 142,
	height: 59,
	imgs: [
		'Concerned.png',
		'Default.png',
		'Disbelief.png',
		'Eating.png',
		'Grimace.png',
		'Sad.png',
		'Scream-Open.png',
		'Serious.png',
		'Smile.png',
		'Tongue.png',
		'Twinkle.png',
		'Vomit.png',
	]
}, {
	name: '衣服',
	path: 'clothing/',
	x: -39,
	y: 192,
	width: 358,
	height: 149,
	imgs: [
		'Blazer-Shirt.png',
		'Blazer-Sweater.png',
		'Collar-Sweater.png',
		'Graphic-Shirt.png',
		'Hoodie.png',
		'Overall.png',
		'Shirt-Crew-Neck.png',
		'Shirt-Scoop-Neck.png',
		'Shirt-V-Neck.png',
	]
}, {
	name: '头发',
	path: 'hair/',
	x: -35,
	y: -28,
	width: 350,
	height: 368,
	imgs: [
		'x',
		'Big-Hair.png',
		'Bob.png',
		'Bun.png',
		'Curly.png',
		'Curvy.png',
		'Dreads-01.png',
		'Dreads-02.png',
		'Dreads.png',
		'Frida.png',
		'Frizzle.png',
		'Fro-Band.png',
		'Fro.png',
		'Long-but-not-too-long.png',
		'Mia-Wallace.png',
		'Shaggy-Mullet.png',
		'Shaggy.png',
		'Shaved-Sides.png',
		'Short-Curly.png',
		'Short-Flat.png',
		'Short-Round.png',
		'Short-Waved.png',
		'Sides.png',
		'Straight-Strand.png',
		'Straight.png',
		'The-Caesar-Side-Part.png',
		'The-Caesar.png',
	]
}, {
	name: '胡子',
	path: 'facial/',
	x: 29,
	y: 78,
	width: 222,
	height: 200,
	lock: true,
	imgs: [
		'x',
		'Beard-Light.png',
		'Beard-Magestic.png',
		'Beard-Medium.png',
		'Moustache-Fancy.png',
		'Moustache-Magnum.png',
	]
}];
export const colors = [{
	name: '背景',
	list: [
		'#b5aef1',
		'#b2dccc',
		'#78a5cc',
		'#506a81',
		'#4a4b94',
		'#1cce90',
		'#d7a3df',
		'#78d7e5',
		'#8591c5',
		'#ff9292',
		'#337b8a',
		'#299f6e',
		'#ffffff'
	]
}, {
	name: '肤色',
	list: [
		'#edb98a',
		'#ffdbb4',
		'#ae5d29',
	]
}];
export const shape = {
	bg_color: 0,
	skin_color: 1,
	eyebrow: 2,
	eye: 3,
	nose: 4,
}
