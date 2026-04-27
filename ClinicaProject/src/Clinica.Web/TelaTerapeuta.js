window.onload = function() {
    const el = {
        dataInput: document.getElementById("dataGestao"),
        grid: document.getElementById("gridGestao"),
        listaGeral: document.getElementById("listaGeralPacientes")
    };

    // Grade de 30 em 30 minutos (10h às 18h)
    const gradeBase = [
        "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", 
        "13:30", "14:00", "14:30", "15:00", "15:30", "16:00", "16:30", 
        "17:00", "17:30", "18:00"
    ];

    // Define data inicial como hoje
    el.dataInput.value = new Date().toISOString().split('T')[0];

    const atualizarTudo = () => {
        const agendaGlobal = JSON.parse(localStorage.getItem('agendaFisioData')) || {};
        const dataSelecionada = el.dataInput.value;

        // 1. RENDERIZAR GRID DE GESTÃO (BLOQUEIOS)
        if (!agendaGlobal[dataSelecionada]) {
            agendaGlobal[dataSelecionada] = {};
            gradeBase.forEach(h => agendaGlobal[dataSelecionada][h] = { status: "disponivel", paciente: null });
        }

        el.grid.innerHTML = "";
        gradeBase.forEach(hora => {
            const info = agendaGlobal[dataSelecionada][hora];
            const isBloqueado = info.status === "bloqueado";
            
            const btn = document.createElement("button");
            btn.className = `btn-gestao ${isBloqueado ? 'bloqueado' : 'liberado'}`;
            btn.innerHTML = isBloqueado ? `🔒 ${hora}` : `🔓 ${hora}`;

            btn.onclick = () => {
                // Se houver paciente, avisa antes de bloquear
                if (info.paciente && !isBloqueado) {
                    if (!confirm(`Existe um agendamento para ${info.paciente}. Deseja bloquear e remover o paciente?`)) return;
                }
                agendaGlobal[dataSelecionada][hora].status = isBloqueado ? "disponivel" : "bloqueado";
                if (!isBloqueado) agendaGlobal[dataSelecionada][hora].paciente = null;
                
                localStorage.setItem('agendaFisioData', JSON.stringify(agendaGlobal));
                atualizarTudo();
            };
            el.grid.appendChild(btn);
        });

        // 2. RENDERIZAR LISTA GERAL DE PACIENTES (SEM FILTRO DE DATA)
        el.listaGeral.innerHTML = "";
        let temPacientes = false;

        // Varre todas as datas do banco de dados
        Object.keys(agendaGlobal).sort().forEach(data => {
            Object.keys(agendaGlobal[data]).sort().forEach(hora => {
                const info = agendaGlobal[data][hora];
                if (info.paciente) {
                    temPacientes = true;
                    const dataFormatada = data.split('-').reverse().join('/');
                    el.listaGeral.innerHTML += `
                        <div class="paciente-item-geral">
                            <div class="info">
                                <strong>${info.paciente}</strong>
                                <span>${dataFormatada} — ${hora}</span>
                            </div>
                            <div class="status-tag">Confirmado</div>
                        </div>`;
                }
            });
        });

        if (!temPacientes) el.listaGeral.innerHTML = "<p class='vazio'>Nenhum agendamento encontrado.</p>";
    };

    el.dataInput.onchange = atualizarTudo;
    atualizarTudo();
};
