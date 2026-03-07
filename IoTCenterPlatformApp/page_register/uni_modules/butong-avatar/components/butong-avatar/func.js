export default {
	rgbToHsl: function(r, g, b) {
		r /= 255, g /= 255, b /= 255;

		let max = Math.max(r, g, b),
			min = Math.min(r, g, b);
		let h, s, l = (max + min) / 2;

		if (max == min) {
			h = s = 0; // achromatic
		} else {
			let d = max - min;
			s = l > 0.5 ? d / (2 - max - min) : d / (max + min);

			switch (max) {
				case r:
					h = (g - b) / d + (g < b ? 6 : 0);
					break;
				case g:
					h = (b - r) / d + 2;
					break;
				case b:
					h = (r - g) / d + 4;
					break;
			}

			h /= 6;
		}

		return [h, s, l];
	},
	hslToRgb: function(h, s, l) {
		let r, g, b;

		if (s == 0) {
			r = g = b = l; // achromatic
		} else {
			function hue2rgb(p, q, t) {
				if (t < 0) t += 1;
				if (t > 1) t -= 1;
				if (t < 1 / 6) return p + (q - p) * 6 * t;
				if (t < 1 / 2) return q;
				if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
				return p;
			}

			let q = l < 0.5 ? l * (1 + s) : l + s - l * s;
			let p = 2 * l - q;

			r = hue2rgb(p, q, h + 1 / 3);
			g = hue2rgb(p, q, h);
			b = hue2rgb(p, q, h - 1 / 3);
		}

		return [r * 255, g * 255, b * 255];
	},
	//16进制转rgb
	hexToRgb: function(hex) {
		hex = hex.toLowerCase();
		let ret = [];
		for (var i = 1; i < 7; i += 2) {
			ret.push(parseInt('0x' + hex.slice(i, i + 2)));
		}
		return ret;
	},
	//数组随机取值
	randArr: function(arr) {
		return arr[Math.floor(Math.random() * arr.length)];
	},
	//数组随机取索引
	randArrIndex: function(arr) {
		return Math.floor(Math.random() * arr.length);
	},
}
