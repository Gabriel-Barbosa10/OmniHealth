window.onload = function() {
    // 1. MAPEAMENTO DE ELEMENTOS (IDs do seu HTML)
    const el = {
        btnAbrir: document.getElementById("btnAbrir"),
        modalAgenda: document.getElementById("modalAgenda"),
        modalSucesso: document.getElementById("modalSucesso"),
        modalCancel: document.getElementById("modalAvisoCancel"),
        confirmar: document.getElementById("confirmar"),
        btnEntendido: document.getElementById("btnEntendido"),
        statusBox: document.getElementById("statusConsulta"),
        containerHoras: document.getElementById("containerHorarios"),
        inputData: document.getElementById("dataAgendamento"), // Campo de data
        selectTerapeuta: document.getElementById("selectTerapeuta")
    };

    let consultaAtiva = null;
    let horaSelecionada = "";

    // Grade de 30 em 30 minutos (Deve ser a mesma do terapeuta)
    const gradeHorarios = [
        "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", 
        "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", 
        "16:00", "16:30", "17:00", "17:30", "18:00"
    ];

    /* ============================================================
       LOGICA DE ATUALIZAÇÃO DO GRID (SINCRONIA COM TERAPEUTA)
    ============================================================ */
    const atualizarGridPaciente = () => {
        const dataSel = el.inputData.value;
        if (!dataSel) {
            el.containerHoras.innerHTML = "<p style='font-size:0.8rem; color:gray; grid-column:1/-1;'>Selecione a data primeiro...</p>";
            return;
        }

        // Lê o "banco de dados" compartilhado
        const agendaGlobal = JSON.parse(localStorage.getItem('agendaFisioData')) || {};
        const agendaDia = agendaGlobal[dataSel] || {};

        el.containerHoras.innerHTML = "";
        horaSelecionada = ""; // Reseta seleção ao mudar data

        gradeHorarios.forEach(h => {
            const info = agendaDia[h] || { status: "disponivel" };
            const btn = document.createElement("button");
            btn.innerText = h;
            btn.type = "button";
            
            // Aplica estilos baseados no status definido pelo médico
            btn.className = `btn-hora ${info.status}`;
            
            if (info.status === "bloqueado") {
                btn.disabled = true;
            } else {
                btn.onclick = () => {
                    document.querySelectorAll('.btn-hora').forEach(b => b.classList.remove('selecionado'));
                    btn.classList.add('selecionado');
                    horaSelecionada = h;
                };
            }
            el.containerHoras.appendChild(btn);
        });
    };

    // Listeners para atualizar o grid
    el.inputData.onchange = atualizarGridPaciente;

    el.btnAbrir.onclick = () => {
        el.inputData.value = ""; // Limpa data ao abrir
        el.containerHoras.innerHTML = "<p style='font-size:0.8rem; color:gray; grid-column:1/-1;'>Aguardando data...</p>";
        el.modalAgenda.showModal();
    };

    /* ============================================================
       LOGICA DE CONFIRMAÇÃO (SALVAMENTO NO BANCO)
    ============================================================ */
    el.confirmar.onclick = () => {
        const medico = el.selectTerapeuta.value;
        const dataOriginal = el.inputData.value;
        
        if(!medico || !dataOriginal || !horaSelecionada) {
            return alert("Por favor, preencha todos os campos!");
        }

        // 1. CARREGA BANCO, SALVA PACIENTE E BLOQUEIA HORÁRIO
        let agendaGlobal = JSON.parse(localStorage.getItem('agendaFisioData')) || {};
        if (!agendaGlobal[dataOriginal]) agendaGlobal[dataOriginal] = {};

        // Salvando na estrutura que o Terapeuta lê
        agendaGlobal[dataOriginal][horaSelecionada] = {
            status: "bloqueado",
            paciente: "Davi Gusmão" // Nome do cliente logado
        };

        localStorage.setItem('agendaFisioData', JSON.stringify(agendaGlobal));

        // 2. PREPARA DADOS PARA O POPUP DE SUCESSO
        consultaAtiva = { 
            medico, 
            dataOriginal,
            dataExibicao: dataOriginal.split('-').reverse().join('/'), 
            hora: horaSelecionada 
        };

        document.getElementById("dadosConfirmados").innerHTML = `
            <div class="resumo-sucesso">
                <p><strong>Especialista:</strong> ${consultaAtiva.medico}</p>
                <p><strong>Data:</strong> ${consultaAtiva.dataExibicao}</p>
                <p><strong>Horário:</strong> ${consultaAtiva.hora}</p>
            </div>
        `;

        el.modalAgenda.close();
        el.modalSucesso.showModal();
    };

    /* ============================================================
       POS-AGENDAMENTO E CANCELAMENTO
    ============================================================ */
    el.btnEntendido.onclick = () => {
        el.modalSucesso.close();
        el.statusBox.innerHTML = `
            <div class="card-agendado" style="background:#f0fdf4; padding:20px; border-radius:15px; border:1px solid #bbf7d0;">
                <p><strong>Sessão com ${consultaAtiva.medico}</strong></p>
                <p>Dia ${consultaAtiva.dataExibicao} às ${consultaAtiva.hora}</p>
                <button id="btnCancelar" class="btn-cancelar" style="width:100%; margin-top:15px;">Desmarcar Consulta</button>
            </div>
        `;
        el.btnAbrir.style.display = "none";
        document.getElementById("btnCancelar").onclick = () => el.modalCancel.showModal();
    };

    document.getElementById("confirmarCancelamento").onclick = () => {
        // REMOVE DO BANCO AO CANCELAR
        let agendaGlobal = JSON.parse(localStorage.getItem('agendaFisioData')) || {};
        if (agendaGlobal[consultaAtiva.dataOriginal]) {
            agendaGlobal[consultaAtiva.dataOriginal][consultaAtiva.hora] = { status: "disponivel", paciente: null };
        }
        localStorage.setItem('agendaFisioData', JSON.stringify(agendaGlobal));

        consultaAtiva = null;
        el.modalCancel.close();
        el.statusBox.innerHTML = '<p style="color: gray;">Nenhuma sessão agendada.</p>';
        el.btnAbrir.style.display = "block";
    };
};
