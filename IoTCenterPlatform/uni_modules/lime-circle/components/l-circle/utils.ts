// @ts-nocheck
export function isDef(value: unknown): boolean {
  return value !== undefined && value !== null;
}

export function isNumber(value: string) {
  return /^\d+(\.\d+)?$/.test(value);
}

/**
 * 换成 px number
 */

export function toPx(value: string | number) {
	// 如果是数字
	if (typeof value === 'number') {
		return value
	}
	// 如果是字符串数字
	if (isNumber(value)) {
		return Number(value)
	}
	// 如果有单位
	if (typeof value === 'string') {
		const reg = /^-?([0-9]+)?([.]{1}[0-9]+){0,1}(em|rpx|px|%)$/g
		const results = reg.exec(value);
		if (!value || !results) {
			return 0;
		}
		const unit = results[3];
		value = parseFloat(value);
		if (unit === 'rpx') {
			return uni.upx2px(value);
		} 
		if (unit === 'px') {
			return value * 1;
		} 
	}
	return 0
}


export function addUnit(value?: string | number): string | undefined {
  if (!isDef(value)) {
    return undefined;
  }

  value = String(value);
  return isNumber(value) ? `${value}px` : value;
}