/* ============================================================
   LÓGICA DA TELA DO PACIENTE (MEUS AGENDAMENTOS)
   ============================================================ */

const pacienteLogado = "Davi Gusmão";

const elementosPaciente = {
    listaConsultas: document.getElementById("listaConsultas"),
    statusVazio: document.getElementById("statusConsultaVazio"),
    modalCancel: document.getElementById("modalAvisoCancel"),
    confirmarCancelBtn: document.getElementById("confirmarCancelamento"),
    modalSintomas: document.getElementById("modalVerSintomas"),
    textoSintomas: document.getElementById("textoSintomasPaciente")
};

let idParaCancelar = null;

const renderizarConsultasPaciente = () => {
    if (!elementosPaciente.listaConsultas) return;

    // 1. Carrega os dados do LocalStorage
    const consultas = JSON.parse(localStorage.getItem("consultas_fisio")) || [];
    
    // 2. Filtra apenas as consultas deste paciente específico
    const minhasConsultas = consultas.filter(c => c.nome === pacienteLogado);

    // 3. Controle de estado vazio
    if (minhasConsultas.length === 0) {
        elementosPaciente.statusVazio.style.display = "block";
        // Remove cards antigos se houver
        const cardsAntigos = document.querySelectorAll('.card-consulta-item');
        cardsAntigos.forEach(card => card.remove());
        return;
    }

    // Oculta o aviso de vazio e limpa a lista para re-renderizar
    elementosPaciente.statusVazio.style.display = "none";
    // Remove apenas os cards de consulta, mantendo o esqueleto
    document.querySelectorAll('.card-consulta-item').forEach(c => c.remove());

    // 4. Criação dos Cards
    minhasConsultas.forEach(consulta => {
        const card = document.createElement("div");
        card.className = "card-moderno card-consulta-item";
        
        card.innerHTML = `
            <div class="card-body-info">
                <span class="badge">Sessão Confirmada</span>
                <h3 class="card-titulo-sessao" style="margin: 15px 0 10px 0;">${consulta.especialista}</h3>
                <div class="info-linha">
                    <p class="card-detalhe"><strong>📅 Data:</strong> ${consulta.data}</p>
                    <p class="card-detalhe"><strong>🕒 Horário:</strong> ${consulta.hora}</p>
                </div>
            </div>
            <div class="botoes-stack" style="margin-top: 20px; display: flex; flex-direction: column; gap: 10px;">
                <button class="btn-agendar-atalho" style="width: 100%; justify-content: center;" 
                        onclick="verSintomas('${consulta.sintomas}')">
                    📋 Ver Meus Sintomas
                </button>
                <button class="btn-cancelar-sessao" onclick="prepararCancelamento('${consulta.id}')">
                    Desmarcar Sessão
                </button>
            </div>
        `;
        elementosPaciente.listaConsultas.appendChild(card);
    });
};

// --- FUNÇÕES DE INTERAÇÃO ---

window.verSintomas = (texto) => {
    elementosPaciente.textoSintomas.innerText = texto;
    elementosPaciente.modalSintomas.showModal();
};

window.prepararCancelamento = (id) => {
    idParaCancelar = id;
    elementosPaciente.modalCancel.showModal();
};

elementosPaciente.confirmarCancelBtn.onclick = () => {
    if (idParaCancelar) {
        let consultas = JSON.parse(localStorage.getItem("consultas_fisio")) || [];
        
        // Localiza a consulta para liberar o horário na agenda antes de deletar
        const consultaRemovida = consultas.find(c => String(c.id) === String(idParaCancelar));
        
        if (consultaRemovida) {
            // 1. Remove da lista de consultas
            const novasConsultas = consultas.filter(c => String(c.id) !== String(idParaCancelar));
            localStorage.setItem("consultas_fisio", JSON.stringify(novasConsultas));

            // 2. Libera o horário na agenda global para que outros possam marcar
            const agendaGlobal = JSON.parse(localStorage.getItem('agendaFisioData')) || {};
            if (agendaGlobal[consultaRemovida.dataISO]) {
                delete agendaGlobal[consultaRemovida.dataISO][consultaRemovida.hora];
                localStorage.setItem('agendaFisioData', JSON.stringify(agendaGlobal));
            }
        }

        elementosPaciente.modalCancel.close();
        renderizarConsultasPaciente(); // Atualiza a tela instantaneamente
    }
};

// Inicializa ao carregar a página
document.addEventListener("DOMContentLoaded", renderizarConsultasPaciente);
// Escuta mudanças feitas em outras abas e atualiza a interface atual
window.addEventListener('storage', (e) => {
    if (e.key === 'consultas_fisio' || e.key === 'agendaFisioData') {
        // Se estiver na tela do paciente, chama a função de renderizar dele
        if (typeof renderizarConsultasPaciente === 'function') renderizarConsultasPaciente();
        
        // Se estiver no dashboard do terapeuta, chama a função de dashboard
        if (typeof renderizarDashboard === 'function') renderizarDashboard();
        
        // Se estiver na gestão de horários, recarrega a grade
        if (typeof carregarTela === 'function') carregarTela();
    }
});