import * as d3 from 'd3'; // 导入D3.js
export function renderCurves(modelData) {
    // 1. 清空所有曲线容器（避免重复绘制）
    document.querySelectorAll('.curve-container').forEach(container => {
        d3.select(container).selectAll('*').remove();
    });
    modelData.forEach((item, rowIndex) => {
        item.Processes.forEach((process, pIdx) => {
            // 核心修复：兼容 ShuRus（大写S）和 shuRus（小写s），优先取有值的
            const shuRusList = Array.isArray(process.ShuRus) 
                ? process.ShuRus 
                : (Array.isArray(process.shuRus) ? process.shuRus : []);

            // 容错：若当前工序无输入项，跳过
            if (shuRusList.length === 0) return;
            shuRusList.forEach((shuRu, sIdx) => {
                // 1. 找到输入项容器和**有效工序项**
                const inputItem = document.querySelector(`#input-item-${rowIndex}-${pIdx}-${sIdx}`);
                // 关键：筛选带 data-is-process="true" 的工序项
                const processItem = document.querySelector(`#process-item-${rowIndex}-${pIdx}[data-is-process="true"]`);
                const curveContainer = document.querySelector(`.curve-container[id="curve-${rowIndex}-${pIdx}-${sIdx}"]`);
                
                if (!inputItem || !processItem || !curveContainer) return;

                // 2. 计算输入项和工序的位置（基于视口的绝对坐标）
                const inputRect = inputItem.getBoundingClientRect();
                const processRect = processItem.getBoundingClientRect();
                const curveRect = curveContainer.getBoundingClientRect();
                // 3. 曲线起点：输入项右侧中间
                const startX = inputRect.right - curveRect.left;
                const startY = inputRect.top - curveRect.top + inputRect.height / 2;

                // 4. 曲线终点：工序左侧中间
                const endX = processRect.left - curveRect.left;
                const endY = processRect.top - curveRect.top + processRect.height / 2;

                // 5. 贝塞尔曲线控制点（优化曲线弧度）
                const controlX1 = startX + 50;  // 向右的控制点，让曲线先延伸再弯曲
                const controlY1 = startY;
                const controlX2 = endX - 50;    // 向左的控制点，让曲线在终点前弯曲
                const controlY2 = endY;

                // 6. 生成 SVG 路径
                const path = `M ${startX} ${startY} C ${controlX1} ${controlY1}, ${controlX2} ${controlY2}, ${endX} ${endY}`;

                // 7. 绘制曲线
                d3.select(curveContainer)
                .append('svg')
                .attr('width', curveRect.width)
                .attr('height', curveRect.height)
                .append('path')
                .attr('d', path)
                .attr('stroke', '#3DB98F')
                .attr('stroke-width', 1)
                .attr('fill', 'none')
                .attr('stroke-dasharray', '5,5');
            });
        });
    });
}
