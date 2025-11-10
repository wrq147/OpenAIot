export function numberToWords(number) {
  const units = ['', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine'];
  const teens = ['', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen'];
  const tens = ['', 'ten', 'twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];
  const scales = ['', 'thousand', 'million', 'billion'];
  if (number === 0) {
    return 'zero';
  }
  let integerPart = Math.floor(number);
  let decimalPart = Math.round((number - integerPart) * 100);
  let words = '';
  // Convert the integer part
  if (integerPart === 0) {
    words = 'zero';
  }
  else {
    let scaleIndex = 0;
    while (integerPart > 0) {
      const hundreds = Math.floor(integerPart % 1000 / 100);
      const tensAndUnits = integerPart % 100;
      if (hundreds > 0) {
        words = units[hundreds] + ' hundred ' + words;
      }
      if (tensAndUnits > 0) {
        if (tensAndUnits < 10) {
          words = units[tensAndUnits] + ' ' + words;
        } else if (tensAndUnits < 20) {
          words = teens[tensAndUnits % 10] + ' ' + words;
        } else {
          words = tens[Math.floor(tensAndUnits / 10)] + ' ' + units[tensAndUnits % 10] + ' ' + words;
        }
      }
      integerPart = Math.floor(integerPart / 1000);
      scaleIndex++;
      if (integerPart > 0) {
        words = scales[scaleIndex] + ' ' + words;
      }
    }
  }
  // Convert the decimal part
  if (decimalPart > 0) {
    words += ' point ';
    if (decimalPart < 10) {
      words += units[decimalPart];
    } else if (decimalPart < 20) {
      words += teens[decimalPart % 10];
    } else {
      words += tens[Math.floor(decimalPart / 10)] + ' ' + units[decimalPart % 10];
    }
  }
  return words.trim()+' yuan';
}
// // Example usage
// const number = 1234567.89;
// const words = numberToWords(number);
// console.log(words); // Output: "one million two hundred thirty four thousand five hundred sixty seven and eighty nine cents"