const refreshButton = document.querySelector('#refresh-metrics');
const statusLabel = document.querySelector('#status');

if (refreshButton && statusLabel) {
  refreshButton.addEventListener('click', () => {
    statusLabel.textContent = `Última actualització: ${new Date().toLocaleString()}`;
  });
}
