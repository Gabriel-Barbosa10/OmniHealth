window.onload = function() {
    // 1. MAPEAMENTO DE ELEMENTOS
    const el = {
        btnAbrir: document.getElementById("btnAbrir"),
        btnConfirmar: document.getElementById("confirmarAgendamento"),
        btnFechar: document.getElementById("btnFechar"),
        
        modalAgenda: document.getElementById("modalAgenda"),
        modalSucesso: document.getElementById("modalSucesso"),
        
        selectPrincipal: document.getElementById('selectTerapeutaPrincipal'),
        selectModal: document.getElementById('selectTerapeuta'),
        inputData: document.getElementById('dataAgendamento'),
        containerHorarios: document.getElementById('containerHorarios'),
        
        toast: document.getElementById("toastErro"),
        msgErro: document.getElementById("mensagemErro"), 
        
        dadosConfirmados: document.getElementById("dadosConfirmados")
    };

    let horaSelecionada = "";

    /* ============================================================
       FUNÇÕES DE APOIO
    ============================================================ */

    // Exibe o erro nítido dentro do modal (ou fora se o modal estiver fechado)
    const mostrarErro = (mensagem) => {
        el.msgErro.innerText = mensagem;
        el.toast.classList.add('toast-visible');
        
        // Remove automaticamente após 3.5 segundos
        setTimeout(() => {
            el.toast.classList.remove('toast-visible');
        }, 3500);
    };

    // Gera botões de horários dinamicamente (Contexto Fisioterapia)
    const atualizarHorarios = () => {
        const medico = el.selectModal.value;
        const data = el.inputData.value;

        if (!medico || !data) {
            el.containerHorarios.innerHTML = `
                <p style="grid-column: 1/-1; text-align:center; font-size:0.85rem; color:var(--text-light); padding:20px;">
                    Escolha o fisioterapeuta e a data para ver a disponibilidade.
                </p>`;
            return;
        }

        el.containerHorarios.innerHTML = "";
        const listaHoras = ["08:00", "09:30", "11:00", "14:00", "15:30", "17:00"];

        listaHoras.forEach(h => {
            const btn = document.createElement("button");
            btn.className = "btn-hora";
            btn.innerText = h;
            btn.type = "button";
            btn.onclick = () => {
                document.querySelectorAll('.btn-hora').forEach(b => b.classList.remove('selecionado'));
                btn.classList.add('selecionado');
                horaSelecionada = h;
            };
            el.containerHorarios.appendChild(btn);
        });
    };

    /* ============================================================
       EVENTOS PRINCIPAIS
    ============================================================ */

    // ABRIR MODAL: Validação na tela principal
    el.btnAbrir.onclick = () => {
        if (!el.selectPrincipal.value) {
            mostrarErro("Selecione um especialista para agendar a sessão.");
            return;
        }
        el.selectModal.value = el.selectPrincipal.value;
        atualizarHorarios();
        el.modalAgenda.showModal();
    };

    // FECHAR MODAL: Reset de estado
    el.btnFechar.onclick = () => {
        el.modalAgenda.close();
        horaSelecionada = ""; 
        el.toast.classList.remove('toast-visible'); // Garante que o erro suma ao fechar
    };

    // Listeners de mudança no formulário
    el.selectModal.onchange = () => {
        horaSelecionada = ""; // Reseta hora ao trocar médico
        atualizarHorarios();
    };
    el.inputData.onchange = atualizarHorarios;

    // CONFIRMAR: Validação rigorosa dentro do Modal
    el.btnConfirmar.onclick = () => {
        const medico = el.selectModal.value;
        const data = el.inputData.value;

        // Verificando campos vazios passo a passo
        if (!medico) {
            mostrarErro("Escolha o fisioterapeuta responsável.");
            return;
        }
        if (!data) {
            mostrarErro("Defina uma data para a sua reabilitação.");
            return;
        }
        if (!horaSelecionada) {
            mostrarErro("Selecione um horário para confirmar.");
            return;
        }

        // SE TUDO OK: Prepara o sucesso
        const nomePaciente = document.getElementById('nomePaciente').innerText;
        const dataFormatada = data.split('-').reverse().join('/');

        el.dadosConfirmados.innerHTML = `
            <div class="resumo-sucesso">
                <p style="margin-bottom:10px;"><strong>👤 Paciente</strong><br>${nomePaciente}</p>
                <p style="margin-bottom:10px;"><strong>🩺 Especialista</strong><br>${medico}</p>
                <p><strong>📅 Agendado para</strong><br>${dataFormatada} às ${horaSelecionada}</p>
            </div>
        `;

        el.modalAgenda.close();
        el.modalSucesso.showModal();
    };
};
el.dadosConfirmados.innerHTML = `
    <div class="resumo-item">
        <span class="icon">👤</span>
        <div>
            <strong>Paciente</strong>
            <p>${nomePaciente}</p>
        </div>
    </div>
    <div class="resumo-item">
        <span class="icon">🩺</span>
        <div>
            <strong>Fisioterapeuta</strong>
            <p>${medico}</p>
        </div>
    </div>
    <div class="resumo-item">
        <span class="icon">📅</span>
        <div>
            <strong>Data e Horário</strong>
            <p>${dataFormatada} às ${horaSelecionada}</p>
        </div>
    </div>
`;
// Dentro da função el.btnConfirmar.onclick
el.btnConfirmar.onclick = () => {
    // ... suas validações de data e hora ...

    if (passouNaValidacao) {
        // 1. Primeiro fecha o modal de agendamento
        el.modalAgenda.close();

        // 2. Preenche os dados (seu código de innerHTML aqui)
        
        // 3. Abre o modal de sucesso como POPUP real
        el.modalSucesso.showModal(); 
    }
};
